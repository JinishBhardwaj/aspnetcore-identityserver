using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer.Configuration
{
public static class Config
{
    public static IEnumerable<ApiScope> Scopes() =>
        new List<ApiScope> {new ApiScope("weatherapi", "Full access to weather api")};

    public static IEnumerable<ApiResource> Apis() =>
        new List<ApiResource>
        {
            new ApiResource("weatherapi", "Weather Service"){Scopes = {"weatherapi"}}
        };

    public static IEnumerable<IdentityResource> Resources() =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(), 
            new IdentityResources.Profile()
        };

    public static IEnumerable<Client> Clients(string apiUrl) =>
        new List<Client>
        {
            new Client
            {
                ClientId = "weatherapi_swagger",
                ClientName = "Weather API Swagger UI",

                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false,

                RedirectUris = {$"{apiUrl}/swagger/oauth2-redirect.html"},
                AllowedCorsOrigins = {$"{apiUrl}"},
                PostLogoutRedirectUris = {$"{apiUrl}/swagger/"},
                AllowedScopes = { "weatherapi" }
            }
        };
}
}
