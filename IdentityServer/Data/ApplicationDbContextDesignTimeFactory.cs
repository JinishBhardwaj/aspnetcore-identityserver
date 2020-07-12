using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace IdentityServer.Data
{
    public class ApplicationDbContextDesignTimeFactory: IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        private const string AspNetCoreEnvironment = "ASPNETCORE_ENVIRONMENT";

        #region Implementation of IDesignTimeDbContextFactory<out ApplicationDbContext>

        /// <inheritdoc />
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var env = Environment.GetEnvironmentVariable(AspNetCoreEnvironment);
            var basePath = Directory.GetCurrentDirectory() + string.Format("{0}..{0}IdentityServer", Path.DirectorySeparatorChar);

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config.GetConnectionString("IdentityDbConnectionName");
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString,
                options => options.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            return new ApplicationDbContext(optionsBuilder.Options);
        }

        #endregion
    }
}
