using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoneyManagerService.Models.Settings;
using MoneyManagerService.Services;
using Polly;

namespace MoneyManagerService.ApplicationStartup.ServiceCollectionExtensions
{
    public static class TaxeeServiceCollectionExtensions
    {
        public static IServiceCollection AddTaxeeServices(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<TaxeeSettings>(config.GetSection("Taxee"));

            var settings = config.GetSection("Taxee").Get<TaxeeSettings>();

            services.AddHttpClient<TaxeeService>()
                .AddTransientHttpErrorPolicy(p =>
                    p.WaitAndRetryAsync(settings.RequestRetryAttempts, _ => TimeSpan.FromMilliseconds(300)));

            return services;
        }
    }
}