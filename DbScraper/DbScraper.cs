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

            var oldRecords = _productionDbContext.Records.Where(r => r.CreatedDate < DateTime.Now.AddMonths(-3)).OrderBy(r => r.Id).ToList();
            _logger.LogInformation($"Found {oldRecords.Count} old records. Here are the old records.");

            foreach (var record in oldRecords)
            {
                _logger.LogInformation($"Old record: {record.Id} - {record.Data} - {record.CreatedDate}");
            }

            var allRecords = _productionDbContext.Records.OrderBy(r => r.Id).ToList();
            foreach (var record in allRecords)
            {
                _logger.LogInformation($"Record: {record.Id} - {record.Data} - {record.CreatedDate}");
            }
        }
    }
}
