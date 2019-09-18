using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.UserData;
using Amphora.Common.Extensions;

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
                Filter = $"geo.distance({nameof(AmphoraExtendedModel.GeoLocation)}, geography'POINT({lat} {lon})') le {dist}"
            };
        }
        public static SearchParameters ByOrganisation(string orgId)
        {
            return new SearchParameters
            {
                Filter = $"{nameof(Entity.OrganisationId)} eq '{orgId}' AND {nameof(Entity.EntityType)} eq '{typeof(OrganisationModel).GetEntityPrefix()}'"
            };
        }
        public static SearchParameters AllPurchased(string userId)
        {
            var p = nameof(ApplicationUserReference.Id);
            return new SearchParameters
            {
                // tags/any(t: t eq 'wifi')
                Filter = $"{nameof(AmphoraSecurityModel.HasPurchased)}/any(h: h/{p} eq '{userId}')"
            };
        }
    }
}