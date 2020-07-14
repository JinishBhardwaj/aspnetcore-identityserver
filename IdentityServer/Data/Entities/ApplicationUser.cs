using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Data.Entities
{
    /// <summary>
    /// Provides a custom implementation of an Asp.Net identity user.
    /// Here we can provide additional custom properties
    /// </summary>
    public class ApplicationUser: IdentityUser { }
}
