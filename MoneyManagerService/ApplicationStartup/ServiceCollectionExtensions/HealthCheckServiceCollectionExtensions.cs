using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MoneyManagerService.Data;

namespace MoneyManagerService.ApplicationStartup.ServiceCollectionExtensions
{
    public static class HealthCheckServiceCollectionExtensions
    {
        public static IServiceCollection AddHealthCheckServices(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddDbContextCheck<DataContext>(
                    name: "Database",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "db" });

            return services;
        }
    }
}