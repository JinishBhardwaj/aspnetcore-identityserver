using IdentityServer.Data;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace IdentityServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = GetConfiguration();
            Log.Logger = CreateLogger(configuration);

            try
            {
                Log.Information("Starting configuring the host...");
                var host = CreateHostBuilder(args).Build();

                Log.Information("Starting applying database migrations...");
                using var scope = host.Services.CreateScope();

                var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await MigrateDatabaseAsync(applicationDbContext,
                    async () => await new ApplicationDataSeeder().SeedAsync(applicationDbContext));

                var configurationContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                await MigrateDatabaseAsync(configurationContext,
                    async () => await new ConfigurationDataSeeder().SeedAsync(configurationContext, configuration));

                var persistedGrantDbContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                await MigrateDatabaseAsync(persistedGrantDbContext, () => Task.CompletedTask);

                Log.Information("Starting the host...");
                await host.RunAsync();
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Program terminated unexpectedly");
                throw;
            }
            finally { Log.CloseAndFlush(); }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog();

        #region Helpers

        /// <summary>
        /// Creates the application configuration container
        /// </summary>
        /// <returns><see cref="IConfiguration"/></returns>
        private static IConfiguration GetConfiguration() =>
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();

        /// <summary>
        /// Creates the application logger with the provided configuration
        /// </summary>
        /// <param name="configuration">Application configuration provider</param>
        /// <returns><see cref="ILogger"/></returns>
        private static ILogger CreateLogger(IConfiguration configuration) =>
            new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Console()
                .WriteTo.File(new JsonFormatter(), $"{configuration.GetValue<string>("LogFileFullName")}", shared: true, rollingInterval: RollingInterval.Day)
                .CreateLogger();

        /// <summary>
        /// Migrates the database and runs the data seeder
        /// </summary>
        /// <typeparam name="TContext">Type of data context</typeparam>
        /// <param name="context">Database context</param>
        /// <param name="seeder">Data seeder action to run</param>
        /// <returns>Task object representing the asynchronous operation</returns>
        private static async Task MigrateDatabaseAsync<TContext>(TContext context, Func<Task> seeder)
            where TContext : DbContext
        {
            await context.Database.MigrateAsync();
            await seeder();
        }

        #endregion
    }
}
