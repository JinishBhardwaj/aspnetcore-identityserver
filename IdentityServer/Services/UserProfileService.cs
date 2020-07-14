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
    /// <summary>
    /// Provides an implementation of the <see cref="IProfileService"/> to add custom claims
    /// to the user. These claims will be available in the access token
    /// </summary>
    public class UserProfileService: IProfileService
    {
        #region Fields

        private readonly UserManager<ApplicationUser> _userManager;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileService"/> class
        /// </summary>
        /// <param name="userManager">Asp.Net Identity user manager</param>
        public UserProfileService(UserManager<ApplicationUser> userManager) =>
            _userManager = userManager;

        #endregion

        /// <inheritdoc /> 
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));
            var subClaimValue = subject.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;

            var user = await _userManager.FindByIdAsync(subClaimValue);
            if (user == null)
                throw new ArgumentException("Invalid sub claim");

            // Note: Here we can add additional user claims as required
            // These claims will be available in the access token generated
            context.IssuedClaims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Id),
                new Claim(JwtClaimTypes.PreferredUserName, user.UserName),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };
        }

        /// <inheritdoc /> 
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
