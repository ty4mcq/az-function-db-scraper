using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace DbScraper
{
    public class DbScraper
    {
        private readonly ILogger _logger;

        public DbScraper(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DbScraper>();
        }

        [Function("DbScraper")]
        public void Run([TimerTrigger("* * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }

            Console.WriteLine("This has temporarily been changed from running once every 3 months to every minute for testing purposes.");

            //Starting new SQL connection
            using(var productionConn = new SqlConnection())
            {
                //Retreiving productionuction connection string from environment variables in local.settings.json
                productionConn.ConnectionString = Environment.GetEnvironmentVariable("productionDb");
                productionConn.Open();

                Console.WriteLine("Success");

                //Defining scrape query to find records older than 3 months and executing query
                var scrapeQuery = @"SELECT *
                FROM Records
                WHERE CreatedDate < DATEADD(month, -3, GETDATE())";

                var scrapeCmd = new SqlCommand(scrapeQuery, productionConn);
                var scrapeReader = scrapeCmd.ExecuteReader();

                //Outputting read data
                while (scrapeReader.Read())
                {
                    _logger.LogInformation($"productionuction Record: {scrapeReader["Data"]}, Created: {scrapeReader["CreatedDate"]}");
                }
            }
        }
    }
}
