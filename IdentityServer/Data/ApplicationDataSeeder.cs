using IdentityServer.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Data
{
    /// <summary>
    /// Provides methods to seed sample data to Asp.Net Identity related tables
    /// </summary>
    public class ApplicationDataSeeder
    {
        #region Fields

        private readonly IPasswordHasher<ApplicationUser> _hasher = new PasswordHasher<ApplicationUser>();

        #endregion

        #region Methods

        /// <summary>
        /// Inserts a default user to the database
        /// </summary>
        /// <param name="context">Database context</param>
        /// <returns>Task object representing the asynchronous operation</returns>
        public async Task SeedAsync(ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                var user = new ApplicationUser
                {
                    UserName = "test@user.com",
                    NormalizedUserName = "TEST@USER.COM",
                    Email = "test@user.com",
                    NormalizedEmail = "TEST@USER.COM",
                    EmailConfirmed = true,
                    PhoneNumber = "1212121212",
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };
                user.PasswordHash = _hasher.HashPassword(user, "Test123@");

                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
            }
        }

        #endregion
    }
}
