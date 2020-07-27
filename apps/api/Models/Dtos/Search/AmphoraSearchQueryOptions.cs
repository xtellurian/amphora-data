using System.Collections.Generic;
using Amphora.Api.Models.Dtos.Platform;

namespace Amphora.Api.Models.Dtos.Search
{
    public class AmphoraSearchQueryOptions : PaginatedResponse
    {
        public override IDictionary<string, string> ToRouteData()
        {
            return new Dictionary<string, string>(base.ToRouteData())
            {
                { nameof(Term), Term },
                { nameof(Labels), Labels },
                { nameof(OrgId), OrgId },
                { nameof(Lat), Lat?.ToString() },
                { nameof(Lon), Lon?.ToString() },
                { nameof(Dist), Dist?.ToString() }
            };
        }

        /// <summary>
        /// Gets or sets the free text search term.
        /// </summary>
        public string Term { get; set; }

        /// <summary>
        /// Gets or sets the comma separated labels that must be included in results.
        /// </summary>
        public string Labels { get; set; }

        /// <summary>
        /// Gets or sets the Organisation ID for the Amphora.
        /// </summary>
        public string OrgId { get; set; }

        /// <summary>
        /// Gets or sets the latitude (center of search area).
        /// </summary>
        public double? Lat { get; set; }

        /// <summary>
        /// Gets or sets the longitude (center of search area).
        /// </summary>
        public double? Lon { get; set; }

        /// <summary>
        /// Gets or sets the distance from center of search area (describing a circle).
        /// </summary>
        public double? Dist { get; set; }
    }
}