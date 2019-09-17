using Amphora.Common.Contracts;
using Amphora.Common.Models;

namespace Amphora.Api.Models.Search
{
    public class SearchParameters : Microsoft.Azure.Search.Models.SearchParameters
    {
        public bool IsForUserAsCreator { get; private set; }
        public static SearchParameters ForUserAsCreator(IApplicationUser user)
        {
            return new SearchParameters()
            {
                IsForUserAsCreator = true,
                Filter = $"{nameof(Entity.CreatedBy)} eq '{user.Id}'"
            };
        }

        public static SearchParameters GeoSearch(double lat, double lon, double dist)
        {
            return new SearchParameters
            {
                Filter = $"geo.distance(Location, geography'POINT({lat} {lon})') le {dist}"
            };
        }
        public static SearchParameters ByOrganisation(string orgId)
        {
            return new SearchParameters
            {
                Filter = $"{nameof(Entity.OrganisationId)} eq '{orgId}'"
            };
        }
    }
}