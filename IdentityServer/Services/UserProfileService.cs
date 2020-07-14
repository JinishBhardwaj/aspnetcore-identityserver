using IdentityModel;
using IdentityServer.Data.Entities;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.String;

namespace IdentityServer.Services
{
    public class UserProfileService: IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfileService(UserManager<ApplicationUser> userManager) =>
            _userManager = userManager;

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));
            var subClaimValue = subject.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;

            var user = await _userManager.FindByIdAsync(subClaimValue);
            if (user == null)
                throw new ArgumentException("Invalid sub claim");

            // Here we can add mode user claims as per requirements
            context.IssuedClaims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Id),
                new Claim(JwtClaimTypes.PreferredUserName, user.UserName),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));
            var subClaimValue = subject.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;

            var user = await _userManager.FindByIdAsync(subClaimValue);
            context.IsActive = false;

            if (user != null)
            {
                if (_userManager.SupportsUserSecurityStamp)
                {
                    var stamp = subject.Claims.FirstOrDefault(x => x.Type == "security_stamp")?.Value;
                    if (!IsNullOrWhiteSpace(stamp))
                    {
                        var securityStampFromDatabase = await _userManager.GetSecurityStampAsync(user);
                        if (stamp != securityStampFromDatabase)
                            return;
                    }
                }

                context.IsActive = !user.LockoutEnabled || !user.LockoutEnd.HasValue || user.LockoutEnd < DateTime.UtcNow;
            }
        }
    }
}
