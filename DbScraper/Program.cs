using DbScraper;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddDbContext<ProductionDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("productionDb")));
        services.AddDbContext<ArchiveDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("archiveDb")));

    })
    .Build();

host.Run();
