using ftrip.io.booking_service.Persistance;
using ftrip.io.framework.Persistence.Sql.Migrations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
