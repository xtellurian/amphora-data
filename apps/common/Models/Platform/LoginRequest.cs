using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Amphora.Common.Models.Platform
{
    public class LoginRequest
    {
        public LoginRequest()
        {
        }

        public LoginRequest(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public LoginRequest(string username, string password, IEnumerable<LoginClaim> claims) : this(username, password)
        {
            Claims = new List<LoginClaim>(claims);
        }

        [Required]
        /// <summary>
        /// Gets or Sets your username for the request.
        /// </summary>
        public string? Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        /// <summary>
        /// Gets or sets your password for the request.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets an optional list of extra scopes you wish to claim.
        /// </summary>
        public List<LoginClaim>? Claims { get; set; } = new List<LoginClaim>();
    }

    public class LoginClaim
    {
        public LoginClaim()
        { }

        public LoginClaim(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public string Type { get; set; } = null!;
        public string? Value { get; set; }
    }
}