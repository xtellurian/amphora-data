using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.ViewModels
{
    public class AmphoraViewModel : MarketEntityViewModel
    {

        [Display(Name = "Content Type")]
        public string ContentType { get; set; }
        [Display(Name = "File Name")]
        public string FileName { get; set; }
    }
}