using System.ComponentModel.DataAnnotations;

namespace Amphora.Common.Models.Dtos
{
    public class RegisterUser
    {
        [Required]
        [Display(Name = "Username")]
        public string? Username { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
