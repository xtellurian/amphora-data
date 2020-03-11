using System.ComponentModel.DataAnnotations;

namespace Amphora.Common.Models.Platform
{
    public class TokenRequest
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}