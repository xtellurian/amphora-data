using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class AmphoraExtendedDto: AmphoraModelDto
    {
        public string Id {get;set;}
        public string OrganisationId {get; set;}
        
        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Display(Name = "Latitude")]
        public double? Lat { get; set; }
        [Display(Name = "Longitude")]
        public double? Lon { get; set; }
    }
}