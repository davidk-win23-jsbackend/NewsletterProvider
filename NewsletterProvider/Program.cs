using Data.Contexts;
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

        var sqlConnectionString = Environment.GetEnvironmentVariable("SqlServer");
        if (string.IsNullOrEmpty(sqlConnectionString))
        {
            throw new InvalidOperationException("Connection string 'SqlServer' not found.");
        }

        services.AddDbContext<DataContext>(options => options.UseSqlServer(sqlConnectionString));
    })
    .Build();

host.Run();