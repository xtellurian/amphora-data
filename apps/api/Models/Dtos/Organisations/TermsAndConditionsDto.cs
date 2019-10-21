using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos
{
    public class TermsAndConditionsDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Contents { get; set; }
    }
}