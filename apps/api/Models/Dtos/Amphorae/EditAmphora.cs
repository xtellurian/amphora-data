using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class EditAmphora : AmphoraDto
    {
        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Display(Name = "Latitude")]
        public double? Lat { get; set; }
        [Display(Name = "Longitude")]
        public double? Lon { get; set; }

        [Display(Name = "Terms and Conditions")]
        public string TermsAndConditionsId { get; set; }
        [Display(Name = "Labels")]
        [RegularExpression(@"^(([-\w ]{0,12})[, ]?){1,6}$", ErrorMessage = "Comma Separated Labels, Max 5 Labels")]
        public string Labels { get; set; }
    }
}