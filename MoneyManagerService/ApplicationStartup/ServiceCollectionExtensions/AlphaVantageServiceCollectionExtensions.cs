using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoneyManagerService.Models.Settings;
using MoneyManagerService.Services;
using Polly;

namespace MoneyManagerService.ApplicationStartup.ServiceCollectionExtensions
{
    public static class AlphaVantageServiceCollectionExtensions
    {
        public static IServiceCollection AddAlphaVantageServices(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<AlphaVantageSettings>(config.GetSection("AlphaVantage"));
            services.AddHttpClient<AlphaVantageService>()
                .AddTransientHttpErrorPolicy(p =>
                    p.WaitAndRetryAsync(config.GetSection("AlphaVantage:RequestRetryAttempts").Get<int>(), _ => TimeSpan.FromMilliseconds(300)));

            return services;
        }
    }
}