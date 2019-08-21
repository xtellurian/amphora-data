using System;

namespace Amphora.Api.Models
{
    public class SearchParams
    {
        public SearchParams()
        {

        }

        public Predicate<double> PriceFilter { get; set; } = (p) => true;
    }
}