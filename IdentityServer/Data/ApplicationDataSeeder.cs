using IdentityServer.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Data
{
    public class ApplicationDataSeeder
    {
        private readonly IPasswordHasher<ApplicationUser> _hasher = new PasswordHasher<ApplicationUser>();

        public async Task SeedAsync(ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                var user = new ApplicationUser
                {
                    UserName = "test@user.com",
                    Email = "test@user.com",
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
    }
}
