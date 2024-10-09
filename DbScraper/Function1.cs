using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace DbScraper
{
    public class Function1
    {
        private readonly ILogger _logger;

        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        [Function("Function1")]
        public void Run([TimerTrigger("* * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }

            Console.WriteLine("This has temporarily been changed from running once every 3 months to every minute for testing purposes.");

            //Starting new SQL connection
            using(var prodConn = new SqlConnection())
            {
                //Retreiving production connection string from environment variables in local.settings.json
                prodConn.ConnectionString = Environment.GetEnvironmentVariable("prodDb");
                prodConn.Open();

                Console.WriteLine("Success");

                //Defining scrape query to find records older than 3 months and executing query
                var scrapeQuery = @"SELECT *
                FROM Records
                WHERE CreatedDate < DATEADD(month, -3, GETDATE())";

                var scrapeCmd = new SqlCommand(scrapeQuery, prodConn);
                var scrapeReader = scrapeCmd.ExecuteReader();

                //Outputting read data
                while (scrapeReader.Read())
                {
                    _logger.LogInformation($"Production Record: {scrapeReader["Data"]}, Created: {scrapeReader["CreatedDate"]}");
                }
            }
        }
    }
}
