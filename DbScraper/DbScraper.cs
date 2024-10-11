using System;
using System.Drawing.Printing;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace DbScraper
{
    public class DbScraper
    {
        private readonly ILogger _logger;
        private readonly ProductionDbContext _productionDbContext;
        private readonly ArchiveDbContext _archiveDbContext;

        // Injecting the logger and the database contexts
        public DbScraper(ILoggerFactory loggerFactory, ProductionDbContext productionDbContext, ArchiveDbContext archiveDbContext)
        {
            _logger = loggerFactory.CreateLogger<DbScraper>();
            _productionDbContext = productionDbContext;
            _archiveDbContext = archiveDbContext;
        }

        [Function("DbScraper")]
        public void Run([TimerTrigger("* * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }

            Console.WriteLine("This has temporarily been changed from running once every week to every minute for testing purposes.");

            // Get all records that are older than 3 months
            var oldRecords = (from r in _productionDbContext.Records
                              where r.CreatedDate < DateTime.Now.AddMonths(-3)
                              orderby r.CreatedDate
                              select r).ToList();

            _logger.LogInformation($"Found {oldRecords.Count} old records. Here are the old records.");

            // Create a new record in the archive database for each old record
            foreach (var  record in oldRecords)
            {
                var archiveRecord = new ArchiveRecord
                {
                    Data = record.Data,
                    CreatedDate = record.CreatedDate
                };

                // Add the record to the archive database
                _archiveDbContext.Records.Add(archiveRecord);
            }

            // Save the changes to the archive database
            _archiveDbContext.SaveChanges();

            _logger.LogInformation("Successfully archived old records.");

        }
    }
}
