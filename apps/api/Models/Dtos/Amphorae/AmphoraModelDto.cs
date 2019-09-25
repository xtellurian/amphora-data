using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class AmphoraModelDto
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        [DataType(DataType.Currency)]
        public double Price { get; set; }


    }
}