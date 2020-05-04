using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class EditAmphora : BasicAmphora
    {
        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Display(Name = "Latitude")]
        public double? Lat { get; set; }
        [Display(Name = "Longitude")]
        public double? Lon { get; set; }

        [Display(Name = "Terms of Use Id")]
        public string TermsOfUseId { get; set; }
        [Display(Name = "Files MetaData")]
        public Dictionary<string, AttributeStore> FileAttributes { get; set; } = new Dictionary<string, AttributeStore>();
    }
}