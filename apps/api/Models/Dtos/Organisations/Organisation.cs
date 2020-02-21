using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Organisations
{
    public class Organisation : Entity
    {
        [DataType(DataType.Text)]
        [Required]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        [Required]
        public string About { get; set; }
        [DataType(DataType.Url)]
        [Display(Name = "Website URL")]
        [Required]
        public string WebsiteUrl { get; set; }
        [DataType(DataType.Text)]
        public string Address { get; set; }
    }
}