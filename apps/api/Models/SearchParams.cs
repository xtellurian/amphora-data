using System;

namespace Amphora.Api.Models
{
    public class SearchParams
    {
        public SearchParams()
        {

        }
        public string SearchTerm { get; set; }
        public Predicate<double> PriceFilter { get; set; } = (p) => true;
        public bool IsGeoSearch { get; set; }
        public string GeoHashStartsWith { get; set; }
    }
}
