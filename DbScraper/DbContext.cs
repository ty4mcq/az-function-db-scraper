using Microsoft.EntityFrameworkCore;

namespace DbScraper
{
    public class ProductionDbContext : DbContext
    {
        public DbSet<ProductionRecord> Records { get; set; }

        public ProductionDbContext(DbContextOptions<ProductionDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("productionDb"));
            }
        }
    }

    public class ArchiveDbContext : DbContext
    {
        public DbSet<ArchiveRecord> Records { get; set; }

        public ArchiveDbContext(DbContextOptions<ArchiveDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("archiveDb"));
            }
        }
    }

    public class ProductionRecord
    {
        public int Id { get; set; }
        public string? Data { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class ArchiveRecord
    {
        public int Id { get; set; }
        public string? Data { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
