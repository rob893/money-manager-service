using System.Reflection;
using MoneyManagerService.ApplicationStartup.ServiceCollectionExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MoneyManagerService.ApplicationStartup.ApplicationBuilderExtensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using AutoMapper;
using Microsoft.AspNetCore.HttpOverrides;
using MoneyManagerService.Middleware;
using MoneyManagerService.Services;

namespace MoneyManagerService.ApplicationStartup
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllerServices()
                .AddLogging()
                .AddDatabaseServices(Configuration)
                .AddAuthenticationServices(Configuration)
                .AddIdentityServices()
                .AddRepositoryServices()
                .AddAlphaVantageServices(Configuration)
                .AddTaxeeServices(Configuration)
                .AddHangfireServices(Configuration)
                .AddSwaggerServices(Configuration)
                .AddAutoMapper(typeof(Startup))
                .AddHealthCheckServices()
                .AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler(builder => builder.UseMiddleware<GlobalExceptionHandlerMiddleware>())
                .UseHsts()
                .UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                })
                .UseCors(header =>
                    header.WithOrigins(Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "*" })
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders(Configuration.GetSection("Cors:ExposedHeaders").Get<string[]>() ?? new[] { "X-Token-Expired", "X-Correlation-Id" })
                )
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseAndConfigureSwagger(env)
                .UseAndConfigureHangfire(Configuration)
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                    {
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    });
                    endpoints.MapControllers();
                });
        }
    }
}
