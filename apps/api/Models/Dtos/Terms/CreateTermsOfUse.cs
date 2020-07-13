using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Terms
{
    public class CreateTermsOfUse
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Contents { get; set; }
    }
}