using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer.Configuration
{
    /// <summary>
    /// Defines identity clients, resources and protected apis details
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Adds the available scopes for the apis
        /// </summary>
        /// <returns>List of scopes</returns>
        public static IEnumerable<ApiScope> Scopes() =>
            new List<ApiScope> {new ApiScope("weatherapi", "Full access to weather api")};

        /// <summary>
        /// Gets the list of apis defined in the system
        /// </summary>
        /// <returns>List of apis</returns>
        public static IEnumerable<ApiResource> Apis() =>
            new List<ApiResource>
            {
                new ApiResource("weatherapi", "Weather Service"){Scopes = {"weatherapi"}}
            };

        /// <summary>
        /// Gets a list of identity resources for a user. For example
        /// user id, email, profile information etc
        /// </summary>
        /// <returns>List of identity resources</returns>
        public static IEnumerable<IdentityResource> Resources() =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(), 
                new IdentityResources.Profile()
            };

        /// <summary>
        /// Gets the list of approved clients with access to api resources
        /// </summary>
        /// <returns>List of approved clients</returns>
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
