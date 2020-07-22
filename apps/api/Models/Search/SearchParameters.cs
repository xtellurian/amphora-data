using System.Collections.Generic;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Models.Search
{
    public class SearchParameters : Microsoft.Azure.Search.Models.SearchParameters
    {
        public SearchParameters()
        {
            if (Filter == null) { Filter = ""; }
        }

        public string OrganisationId { get; set; }
        private string LabelsProperty => nameof(AmphoraModel.Labels);
        private string IsPublicProperty => nameof(AmphoraModel.IsPublic);
        private string GeoLocationProperty => nameof(AmphoraModel.GeoLocation);
        private string OrganisationIdProperty => nameof(AmphoraModel.OrganisationId);
        private string PurchaseCountPropertyName => nameof(AmphoraModel.PurchaseCount);
        public SearchParameters NotDeleted()
        {
            if (this.Filter.Length > 1) { this.Filter += " and "; }
            Filter += $"{nameof(ISearchable.IsDeleted)} ne true";
            return this;
        }

        public SearchParameters IncludeLabelsFacet<T>() where T : ISearchable
        {
            if (HasProperty<T>(LabelsProperty))
            {
                if (Facets == null) { Facets = new List<string>(); }
                Facets.Add($"{LabelsProperty}/Name");
            }

            return this;
        }

        public SearchParameters OrderByPurchaseCount<T>() where T : ISearchable
        {
            if (HasProperty<T>(PurchaseCountPropertyName))
            {
                OrderBy ??= new List<string>();
                OrderBy.Add(PurchaseCountPropertyName);
            }

            return this;
        }

        public SearchParameters PublicOnly<T>() where T : ISearchable
        {
            if (HasProperty<T>(IsPublicProperty))
            {
                if (this.Filter.Length > 1) { this.Filter += " and "; }
                Filter += $"{IsPublicProperty} eq true";
            }

            return this;
        }

        public SearchParameters WithGeoSearch<T>(double lat, double lon, double dist) where T : ISearchable
        {
            if (HasProperty<T>(GeoLocationProperty))
            {
                if (this.Filter.Length > 1) { this.Filter += " and "; }
                this.Filter += $"geo.distance({GeoLocationProperty}, geography'POINT({lon} {lat})') le {dist}";
            }

            return this;
        }

        public SearchParameters FilterByLabel<T>(IEnumerable<Label> labels) where T : ISearchable
        {
            if (HasProperty<T>(LabelsProperty))
            {
                foreach (var l in labels)
                {
                    if (Filter.Length > 1) { Filter += " and "; }
                    this.Filter += $"{LabelsProperty}/any(t: t/Name eq '{l.Name}') ";
                }
            }

            return this;
        }

        public SearchParameters ForUserAsCreator(IUser user)
        {
            if (Filter.Length > 1) { Filter += " and "; }
            Filter = $"{nameof(ISearchable.CreatedById)} eq '{user.Id}'";
            return this;
        }

        public SearchParameters FilterByOrganisation<T>(string orgId) where T : ISearchable
        {
            if (HasProperty<T>(OrganisationIdProperty))
            {
                if (Filter.Length > 1) { Filter += " and "; }
                Filter = $"{OrganisationIdProperty} eq '{orgId}'";

                this.OrganisationId = orgId;
            }

            return this;
        }

        public SearchParameters WithTotalResultCount()
        {
            this.IncludeTotalResultCount = true;
            return this;
        }

        public static bool HasProperty<T>(string propertyName)
        {
            return typeof(T).GetProperty(propertyName) != null;
        }
    }
}