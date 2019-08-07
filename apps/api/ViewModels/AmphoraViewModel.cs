using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.ViewModels
{
    public class AmphoraViewModel
    {
        public string Id { get; set; }
        public string OrgId { get; set; }
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DataType(DataType.Currency)]
        public double Price { get; set; }

        [Display(Name = "Content Type")]
        public string ContentType { get; set; }
    }
}