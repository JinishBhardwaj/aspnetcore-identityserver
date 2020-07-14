using IdentityServer.Data;
using IdentityServer.Data.Entities;
using IdentityServer.Services;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace IdentityServer
{
    public class Startup
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class
        /// </summary>
        /// <param name="configuration">Application configuration</param>
        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

        #endregion

        private IConfiguration Configuration { get;}

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Service collection</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => AddDbContext(options, Configuration));
            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            services.AddIdentityServer()
                    .AddDeveloperSigningCredential()
                    .AddAspNetIdentity<ApplicationUser>()
                    .AddConfigurationStore(options =>
                    {
                        options.ConfigureDbContext = builder => AddDbContext(builder, Configuration);
                    })
                    .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = builder => AddDbContext(builder, Configuration);
                    })
                    .Services.AddTransient<IProfileService, UserProfileService>();
                    
            services.AddControllers();
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application configuration provider</param>
        /// <param name="env">Application hosting environment</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
            });
        }

        #region Helpers

        /// <summary>
        /// Configures the database options
        /// </summary>
        /// <param name="builder">Database options builder</param>
        /// <param name="configuration">Application configuration provider</param>
        /// <returns>Configured database options builder</returns>
        private static DbContextOptionsBuilder AddDbContext(DbContextOptionsBuilder builder,
            IConfiguration configuration) =>
            builder.UseSqlServer(configuration.GetConnectionString("IdentityDbConnectionName"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(Startup).Assembly.FullName);
                    sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });

        #endregion
    }
}
