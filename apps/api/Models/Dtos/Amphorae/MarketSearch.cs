using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class MarketSearch
    {
        public string Term { get; set; }
        public int? Skip { get; set; }
        public int? Top { get; set; }

        [Display(Name = "Latitude")]
        public double? Lat { get; set; }

        [Display(Name = "Longitude")]
        public double? Lon { get; set; }
        [Display(Name = "Distance")]
        public double? Dist { get; set; }
    }
}
