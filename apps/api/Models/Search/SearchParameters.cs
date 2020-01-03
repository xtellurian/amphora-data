using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using System;
using Amphora.Common.Models.Users;
using System.Collections.Generic;

namespace Amphora.Api.Models.Search
{
    public class SearchParameters : Microsoft.Azure.Search.Models.SearchParameters
    {
        public bool IsForUserAsCreator { get; private set; }
        public SearchParameters WithGeoSearch(double lat, double lon, double dist)
        {
            if (this.Filter == null) this.Filter = "";
            else if (this.Filter.Length > 1) this.Filter += " and ";
            this.Filter += $"geo.distance({nameof(AmphoraModel.GeoLocation)}, geography'POINT({lon} {lat})') le {dist}";
            return this;
        }

        public SearchParameters WithTotalResultCount()
        {
            this.IncludeTotalResultCount = true;
            return this;
        }

        public SearchParameters NotDeleted()
        {
            if (this.Filter == null) Filter = "";
            else if (this.Filter.Length > 1) this.Filter += " and ";
            Filter += $"{nameof(AmphoraModel.IsDeleted)} ne true";
            return this;
        }

        public SearchParameters WithPublicAmphorae()
        {
            if (this.Filter == null) Filter = "";
            else if (this.Filter.Length > 1) this.Filter += " and ";
            Filter += $"{nameof(AmphoraModel.IsPublic)} eq true";
            return this;
        }
        public static SearchParameters ForUserAsCreator(IUser user)
        {
            var p = new SearchParameters()
            {
                IsForUserAsCreator = true,
                Filter = $"{nameof(AmphoraModel.CreatedById)} eq '{user.Id}'"
            };
            return p.NotDeleted();
        }

        public SearchParameters IncludeLabelsFacet()
        {
            if (Facets == null) Facets = new List<string>();
            Facets.Add($"{nameof(AmphoraModel.Labels)}/Name");
            return this;
        }

        public SearchParameters FilterByLabel(params Label[] labels)
        {
            return this.FilterByLabel(labels);
        }
        public SearchParameters FilterByLabel(IEnumerable<Label> labels)
        {
            if (Filter == null) Filter = "";
            foreach (var l in labels)
            {
                if (Filter.Length > 1) Filter += " and ";
                this.Filter += $"{nameof(AmphoraModel.Labels)}/any(t: t/Name eq '{l.Name}') ";
            }
            return this;
        }

        public static SearchParameters GeoSearch(double lat, double lon, double dist)
        {
            return new SearchParameters().WithGeoSearch(lat, lon, dist).NotDeleted();
        }
        public static SearchParameters ByOrganisation(string orgId, Type discriminator)
        {
            return new SearchParameters
            {
                Filter = $"{nameof(AmphoraModel.OrganisationId)} eq '{orgId}' and Discriminator eq '{nameof(discriminator)}'"
            }.NotDeleted();
        }
        public static SearchParameters AllPurchased(string userId)
        {
            var p = nameof(ApplicationUser.Id);
            return new SearchParameters
            {
                // tags/any(t: t eq 'wifi')
                Filter = $"{nameof(AmphoraModel.Purchases)}/any(h: h/{p} eq '{userId}')"
            }.NotDeleted();
        }

        public static SearchParameters PublicAmphorae()
        {
            return new SearchParameters
            {
                Filter = $"{nameof(AmphoraModel.IsPublic)} eq true"
            }.NotDeleted();
        }
    }
}