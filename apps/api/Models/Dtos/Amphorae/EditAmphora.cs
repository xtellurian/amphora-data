using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class EditAmphora : BasicAmphora
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
    }
}