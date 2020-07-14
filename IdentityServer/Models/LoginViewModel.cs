using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models
{
    /// <summary>
    /// Represents a login request from the user
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Gets or sets the users username
        /// <remarks>This the users registered email address</remarks>
        /// </summary>
        [Required]
        [EmailAddress]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the users password
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the url to redirect the user back to, once
        /// the authentication is successfully completed
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
