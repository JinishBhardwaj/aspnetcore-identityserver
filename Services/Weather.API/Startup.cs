using System;
using System.Collections.Generic;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Weather.API.Core.Swagger;

namespace Weather.API
{
    public class Startup
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class
        /// </summary>
        /// <param name="configuration">Application configuration</param>
        /// <param name="environment">Host environment</param>
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        #endregion

        private IConfiguration Configuration { get; }
        private IHostEnvironment Environment { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Service collection</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Weather Forecast API",
                    Version = "v1"
                });
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{Configuration.GetValue<string>("IdentityProviderBaseUrl")}/connect/authorize"),
                            TokenUrl = new Uri($"{Configuration.GetValue<string>("IdentityProviderBaseUrl")}/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                ["weatherapi"] = "Weather API"
                            }
                        }
                    }
                });
                options.OperationFilter<AuthorizeOperationFilter>();
            });
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.ApiName = "weatherapi";
                        options.Authority = Configuration.GetValue<string>("IdentityProviderBaseUrl");
                        options.RequireHttpsMetadata = Environment.IsProduction();
                    });
            services.AddCors(options =>
            {
                options.AddPolicy("DefaultCorsPolicy", corsOptions =>
                {
                    corsOptions.SetIsOriginAllowed(host => true)
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                });
            });
            services.AddControllers()
                    .AddMvcOptions(options => options.Filters.Add(new AuthorizeFilter()));
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

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger()
                .UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather API");
                    options.OAuthClientId("weatherapi_swagger");
                    options.OAuthAppName("Weather API");
                    options.OAuthUsePkce();
                });

            app.UseCors("DefaultCorsPolicy");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
