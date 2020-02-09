using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class DetailedAmphora : BasicAmphora
    {
        public string OrganisationId { get; set; }
        public string TermsAndConditionsId { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Display(Name = "Latitude")]
        public double? Lat { get; set; }
        [Display(Name = "Longitude")]
        public double? Lon { get; set; }
        [Display(Name = "Purchase Count")]
        public int? PurchaseCount { get; set; }
        [Display(Name = "Files MetaData")]
        public Dictionary<string, AttributeStore> FilesMetaData { get; set; } = new Dictionary<string, AttributeStore>();
        [Display(Name = "Signals MetaData")]
        public Dictionary<string, AttributeStore> SignalsMetaData { get; set; } = new Dictionary<string, AttributeStore>();
    }
}