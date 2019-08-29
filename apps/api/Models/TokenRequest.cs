using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models
{
    public class TokenRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}