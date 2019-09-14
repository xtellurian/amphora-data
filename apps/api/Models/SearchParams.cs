using System;
using Newtonsoft.Json;

namespace Amphora.Api.Models
{
    [Obsolete]
    public class SearchParams
    {
        public SearchParams()
        {

        }
        public string SearchTerm { get; set; }

        [JsonIgnore]
        public Predicate<double> PriceFilter { get; set; } = (p) => true;
        public bool IsGeoSearch { get; set; }
        public string GeoHashStartsWith { get; set; }
    }
}
