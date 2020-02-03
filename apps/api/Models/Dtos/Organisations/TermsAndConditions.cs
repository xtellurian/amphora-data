using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Organisations
{
    public class TermsAndConditions
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Contents { get; set; }
    }
}