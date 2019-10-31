using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class AmphoraDto
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public double Price { get; set; }

    }
}