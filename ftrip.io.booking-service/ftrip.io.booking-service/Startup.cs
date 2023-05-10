using ftrip.io.booking_service.Persistance;
using ftrip.io.framework.auth;
using ftrip.io.framework.CQRS;
using ftrip.io.framework.ExceptionHandling.Extensions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.HealthCheck;
using ftrip.io.framework.Installers;
using ftrip.io.framework.Mapping;
using ftrip.io.framework.messaging.Installers;
using ftrip.io.framework.Persistence.Sql.Mariadb;
using ftrip.io.framework.Secrets;
using ftrip.io.framework.Swagger;
using ftrip.io.framework.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            InstallerCollection.With(
                new SwaggerInstaller<Startup>(services),
                new AutoMapperInstaller<Startup>(services),
                new FluentValidationInstaller<Startup>(services),
                new GlobalizationInstaller<Startup>(services),
                new EnviromentSecretsManagerInstaller(services),
                new JwtAuthenticationInstaller(services),
                new MariadbInstaller<DatabaseContext>(services),
                new MariadbHealthCheckInstaller(services),
                new CQRSInstaller<Startup>(services),
                new RabbitMQInstaller<Startup>(services, RabbitMQInstallerType.Publisher | RabbitMQInstallerType.Consumer)
            ).Install();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(policy => policy
                .WithOrigins(Environment.GetEnvironmentVariable("API_PROXY_URL"))
                .AllowAnyMethod()
                .AllowAnyHeader()
            );

            app.UseAuthentication();
            app.UseAuthorization();

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
