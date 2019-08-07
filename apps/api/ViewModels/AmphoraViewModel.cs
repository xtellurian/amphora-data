using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.ViewModels
{
    public class AmphoraViewModel : SearchableDataEntityViewModel
    {

        [Display(Name = "Content Type")]
        public string ContentType { get; set; }
    }
}