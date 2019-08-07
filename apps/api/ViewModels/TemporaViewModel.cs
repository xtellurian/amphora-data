using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.ViewModels
{
    public class TemporaViewModel
    {
        public string Id { get; set; }
        public string OrgId { get; set; }
        
        [Required]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DataType(DataType.Currency)]
        public double Price { get; set; }

    }
}