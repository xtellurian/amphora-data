using System.ComponentModel.DataAnnotations;

namespace Amphora.Common.Models.Dtos
{
    public abstract class BaseAmphoraUser
    {
        protected const string PhoneNumberRegex = @"^\+?([0-9 ]){6,24}$";

        [DataType(DataType.PhoneNumber)]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} numbers long.", MinimumLength = 6)]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [DataType(DataType.MultilineText)]
        [MaxLength(255)]
        [Display(Name = "About")]
        public string? About { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string? UserName { get; set; }
    }
}