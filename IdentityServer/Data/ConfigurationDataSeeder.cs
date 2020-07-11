using IdentityServer.Configuration;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Data
{
    public class ConfigurationDataSeeder
{
    public async Task SeedAsync(ConfigurationDbContext context, IConfiguration configuration)
    {
        var clientUrl = configuration.GetValue<string>("WeatherApiClient");

        if (!context.Clients.Any())
        {
            foreach (var client in Config.Clients(clientUrl))
                await context.Clients.AddAsync(client.ToEntity());

            await context.SaveChangesAsync();
        }
        else
        {
            var oldRedirects = (await context.Clients.Include(c => c.RedirectUris)
                    .ToListAsync())
                    .SelectMany(c => c.RedirectUris)
                    .Where(ru => ru.RedirectUri.EndsWith("/o2c.html"))
                    .ToList();

            if (oldRedirects.Any())
            {
                foreach (var redirectUri in oldRedirects)
                {
                    redirectUri.RedirectUri = redirectUri.RedirectUri.Replace("/o2c.html", "/oauth2-redirect.html");
                    context.Update(redirectUri.Client);
                }
                await context.SaveChangesAsync();
            }
        }

        if (!context.IdentityResources.Any())
        {
            foreach (var resource in Config.Resources())
                await context.IdentityResources.AddAsync(resource.ToEntity());

            await context.SaveChangesAsync();
        }

        if (!context.ApiResources.Any())
        {
            foreach (var api in Config.Apis())
                await context.ApiResources.AddAsync(api.ToEntity());

            await context.SaveChangesAsync();
        }

        if (!context.ApiScopes.Any())
        {
            foreach (var scope in Config.Scopes())
                await context.ApiScopes.AddAsync(scope.ToEntity());

            await context.SaveChangesAsync();
        }
    }
}
}
