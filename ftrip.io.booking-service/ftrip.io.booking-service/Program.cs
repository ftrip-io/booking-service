using ftrip.io.booking_service.Persistance;
using ftrip.io.framework.Logging;
using ftrip.io.framework.Persistence.Sql.Migrations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace ftrip.io.booking_service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .MigrateDbContext<DatabaseContext>()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilogLogging((hostingContext) =>
                {
                    return new LoggingOptions()
                    {
                        ApplicationName = hostingContext.HostingEnvironment.ApplicationName,
                        ApplicationLabel = "booking",
                        ClientIdAttribute = "X-Forwarded-For",
                        GrafanaLokiUrl = Environment.GetEnvironmentVariable("GRAFANA_LOKI_URL")
                    };
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
