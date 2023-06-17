using ftrip.io.booking_service.Installers;
using ftrip.io.booking_service.Persistance;
using ftrip.io.framework.auth;
using ftrip.io.framework.Correlation;
using ftrip.io.framework.CQRS;
using ftrip.io.framework.ExceptionHandling.Extensions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.HealthCheck;
using ftrip.io.framework.Installers;
using ftrip.io.framework.Mapping;
using ftrip.io.framework.messaging.Installers;
using ftrip.io.framework.Metrics;
using ftrip.io.framework.Persistence.Sql.Mariadb;
using ftrip.io.framework.Proxies;
using ftrip.io.framework.Secrets;
using ftrip.io.framework.Swagger;
using ftrip.io.framework.Tracing;
using ftrip.io.framework.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace ftrip.io.booking_service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpClient("catalog", client =>
                client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("CATALOG_SERVICE_URL"))
            );

            InstallerCollection.With(
                new SwaggerInstaller<Startup>(services),
                new HealthCheckUIInstaller(services),
                new AutoMapperInstaller<Startup>(services),
                new FluentValidationInstaller<Startup>(services),
                new GlobalizationInstaller<Startup>(services),
                new EnviromentSecretsManagerInstaller(services),
                new JwtAuthenticationInstaller(services),
                new MariadbInstaller<DatabaseContext>(services),
                new MariadbHealthCheckInstaller(services),
                new CQRSInstaller<Startup>(services),
                new RabbitMQInstaller<Startup>(services, RabbitMQInstallerType.Publisher | RabbitMQInstallerType.Consumer),
                new DependenciesInstaller(services),
                new CorrelationInstaller(services),
                new TracingInstaller(services, (tracingSettings) =>
                {
                    tracingSettings.ApplicationLabel = "booking";
                    tracingSettings.ApplicationVersion = GetType().Assembly.GetName().Version?.ToString() ?? "unknown";
                    tracingSettings.MachineName = Environment.MachineName;
                }),
                new ProxyGeneratorInstaller(services),
                new MetricsInstaller(services)
            ).Install();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseMetrics();

            app.UseCors(policy => policy
                .WithOrigins(Environment.GetEnvironmentVariable("API_PROXY_URL"))
                .AllowAnyMethod()
                .AllowAnyHeader()
            );

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCorrelation();
            app.UseFtripioGlobalExceptionHandler();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseFtripioSwagger(Configuration.GetSection(nameof(SwaggerUISettings)).Get<SwaggerUISettings>());
            app.UseFtripioHealthCheckUI(Configuration.GetSection(nameof(HealthCheckUISettings)).Get<HealthCheckUISettings>());
        }
    }
}