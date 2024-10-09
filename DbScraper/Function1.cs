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
            using(var connection = new SqlConnection())
            {
                //Retreiving connection string from environment variables in local.settings.json
                connection.ConnectionString = Environment.GetEnvironmentVariable("productionDb");
                connection.Open();

                Console.WriteLine("Success");

                //Defining query and executing
                var query = "SELECT * FROM Records";
                var command = new SqlCommand(query, connection);
                var reader = command.ExecuteReader();

                //Outputting read data
                while (reader.Read())
                {
                    _logger.LogInformation($"Production Record: {reader["Data"]}, Created: {reader["CreatedDate"]}");
                }
            }
        }
    }
}
