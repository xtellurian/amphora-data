using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class EditAmphora : BasicAmphora
    {
        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Terms of Use Id")]
        public string TermsOfUseId { get; set; }
    }
}