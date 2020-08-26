using System.Collections.Generic;
using Amphora.Api.Models.Dtos.Platform;

namespace Amphora.Api.Models.Dtos.Search
{
    public class OrganisationSearchQueryOptions : PaginatedResponse
    {
        public override IDictionary<string, string> ToRouteData()
        {
            var routeData = new Dictionary<string, string>(base.ToRouteData());
            AddIfNotNull(routeData, nameof(Term), Term);
            return routeData;
        }

        /// <summary>
        /// Gets or sets the free text search term.
        /// </summary>
        public string Term { get; set; }
    }
}