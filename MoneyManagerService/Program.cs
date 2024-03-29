using System;
using System.IO;
using System.Linq;
using CommandLine;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MoneyManagerService.ApplicationStartup;
using MoneyManagerService.Core;
using MoneyManagerService.Data;

namespace MoneyManagerService
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            if (args.Contains(CommandLineOptions.seedArgument, StringComparer.OrdinalIgnoreCase))
            {
                Parser.Default.ParseArguments<CommandLineOptions>(args)
                    .WithParsed(o =>
                    {
                        var scope = host.Services.CreateScope();
                        var serviceProvider = scope.ServiceProvider;
                        var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();

                        if (o.Password != null && o.Password == GetSeederPasswordFromConfiguration())
                        {
                            var migrate = args.Contains(CommandLineOptions.migrateArgument, StringComparer.OrdinalIgnoreCase);
                            var clearData = args.Contains(CommandLineOptions.clearDataArgument, StringComparer.OrdinalIgnoreCase);
                            var seedData = args.Contains(CommandLineOptions.seedDataArgument, StringComparer.OrdinalIgnoreCase);
                            var dropDatabase = args.Contains(CommandLineOptions.dropArgument, StringComparer.OrdinalIgnoreCase);

                            var seeder = serviceProvider.GetRequiredService<Seeder>();

                            logger.LogInformation($"Seeding database:\nDrop database: {dropDatabase}\nApply Migrations: {migrate}\nClear old data: {clearData}\nSeed new data: {seedData}");
                            logger.LogWarning("Are you sure you want to apply these actions to the database in that order? Only 'yes' will continue.");

                            var answer = Console.ReadLine();

                            if (answer == "yes")
                            {
                                seeder.SeedDatabase(seedData, clearData, migrate, dropDatabase);
                            }
                            else
                            {
                                logger.LogWarning("Aborting database seed process...");
                            }
                        }
                        else
                        {
                            logger.LogWarning("Invalid seeder password");
                        }

                        scope.Dispose();
                    });
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((HostBuilderContext, config) => config.AddJsonFile("appsettings.Secrets.json", false, true))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }

        private static string GetSeederPasswordFromConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Secrets.json", optional: false);

            var config = builder.Build();

            return config.GetValue<string>("SeederPassword");
        }
    }
}
