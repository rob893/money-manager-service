using Microsoft.Extensions.DependencyInjection;
using MoneyManagerService.Data.Repositories;

namespace MoneyManagerService.ApplicationStartup.ServiceCollectionExtensions
{
    public static class RepositoryServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            // Interface => concrete implementation
            services.AddScoped<UserRepository>();

            return services;
        }
    }
}