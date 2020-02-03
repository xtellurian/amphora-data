using System.ComponentModel.DataAnnotations;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class DetailedAmphora : AmphoraDto
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
        public Dictionary<string, MetaDataStore> FilesMetaData { get; set; } = new Dictionary<string, MetaDataStore>();
        [Display(Name = "Signals MetaData")]
        public MetaDataStore SignalsMetaData { get; set; } = new MetaDataStore();
    }
}