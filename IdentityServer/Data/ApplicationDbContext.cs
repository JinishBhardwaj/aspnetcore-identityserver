using IdentityServer.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Data
{
    /// <summary>
    /// Database context for managing user identity
    /// </summary>
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class
        /// </summary>
        /// <param name="options">Database context options</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        #endregion
    }
}
