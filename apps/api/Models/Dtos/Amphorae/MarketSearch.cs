using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class MarketSearch
    {
        public string Term { get; set; }
        public int? Page { get; set; }
        public int? Skip => Top * Page;
        public int? Top { get; set; }

        [Display(Name = "Latitude")]
        public double? Lat { get; set; }

        [Display(Name = "Longitude")]
        public double? Lon { get; set; }
        [Display(Name = "Distance")]
        public double? Dist { get; set; }
        // comma delimited
        public string Labels { get; set; }
    }
}
