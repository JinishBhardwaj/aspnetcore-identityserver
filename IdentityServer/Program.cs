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

                var configurationContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                await configurationContext.Database.EnsureCreatedAsync();
                await new ConfigurationDataSeeder().SeedAsync(configurationContext, configuration);
                await configurationContext.Database.MigrateAsync();

                var persistedGrantDbContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                await persistedGrantDbContext.Database.EnsureCreatedAsync();
                await persistedGrantDbContext.Database.MigrateAsync();

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

        private static IConfiguration GetConfiguration() =>
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();

        private static Serilog.ILogger CreateLogger(IConfiguration configuration) =>
            new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Console()
                .WriteTo.File(new JsonFormatter(), $"{configuration.GetValue<string>("LogFileFullName")}", shared: true, rollingInterval: RollingInterval.Day)
                .CreateLogger();
    }
}
