using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class DetailedAmphora : EditAmphora
    {
        [Display(Name = "Purchase Count")]
        public int? PurchaseCount { get; set; }
        [Display(Name = "Signals Count")]
        public int? SignalCount { get; set; }
    }
}