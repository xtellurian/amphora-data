using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using System;
using Amphora.Common.Models.Users;

namespace Amphora.Api.Models.Search
{
    public class SearchParameters : Microsoft.Azure.Search.Models.SearchParameters
    {
        public bool IsForUserAsCreator { get; private set; }
        public static SearchParameters ForUserAsCreator(IUser user)
        {
            return new SearchParameters()
            {
                IsForUserAsCreator = true,
                Filter = $"{nameof(AmphoraModel.CreatedById)} eq '{user.Id}'"
            };
        }

        public static SearchParameters GeoSearch(double lat, double lon, double dist)
        {
            return new SearchParameters
            {
                // distance in km
                Filter = $"geo.distance({nameof(AmphoraModel.GeoLocation)}, geography'POINT({lon} {lat})') le {dist}"
            };
        }
        public static SearchParameters ByOrganisation(string orgId, Type discriminator)
        {
            return new SearchParameters
            {
                Filter = $"{nameof(AmphoraModel.OrganisationId)} eq '{orgId}' and Discriminator eq '{nameof(discriminator)}'"
            };
        }
        public static SearchParameters AllPurchased(string userId)
        {
            var p = nameof(ApplicationUser.Id);
            return new SearchParameters
            {
                // tags/any(t: t eq 'wifi')
                Filter = $"{nameof(AmphoraModel.Purchases)}/any(h: h/{p} eq '{userId}')"
            };
        }

        public static SearchParameters PublicAmphorae()
        {
            return new SearchParameters
            {
                Filter = $"{nameof(AmphoraModel.IsPublic)} eq true"
            };
        }
    }
}