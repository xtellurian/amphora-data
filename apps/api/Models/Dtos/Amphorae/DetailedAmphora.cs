using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class DetailedAmphora : EditAmphora
    {
        public string OrganisationId { get; set; }

        [Display(Name = "Purchase Count")]
        public int? PurchaseCount { get; set; }
    }
}