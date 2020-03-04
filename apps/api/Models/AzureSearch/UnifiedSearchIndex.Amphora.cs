using System.Collections.Generic;
using Amphora.Common.Models.Amphorae;
using Microsoft.Azure.Search.Models;

namespace Amphora.Api.Models.AzureSearch
{
    // Amphora
    public partial class UnifiedSearchIndex : Index
    {
        protected void AmphoraFields()
        {
            // org id for filtering
            Fields.Add(new Field(nameof(AmphoraModel.OrganisationId), DataType.String)
            {
                IsFacetable = true,
                IsRetrievable = true,
                IsFilterable = true
            });

            // add description
            Fields.Add(new Field(nameof(AmphoraModel.Description), DataType.String)
            {
                IsSearchable = true
            });
            // add description
            Fields.Add(new Field(nameof(AmphoraModel.PurchaseCount), DataType.Int32)
            {
                IsRetrievable = true,
                IsSortable = true
            });
            // add price
            Fields.Add(new Field(nameof(AmphoraModel.Price), DataType.Double)
            {
                IsRetrievable = true,
                IsSortable = true,
                IsFacetable = true,
                IsFilterable = true
            });
            // labels
            Fields.Add(new Field(nameof(AmphoraModel.Labels), DataType.Collection(DataType.Complex),
                new List<Field>
                {
                    new Field(nameof(Label.Name), DataType.String)
                    {
                        IsRetrievable = true,
                        IsSearchable = true,
                        IsFacetable = true,
                        IsFilterable = true
                    }
                }));

            // Location
            Fields.Add(new Field(nameof(AmphoraModel.GeoLocation), DataType.GeographyPoint)
            {
                IsRetrievable = true,
                IsSortable = true,
                IsFilterable = true
            });

            // add isPubic
            Fields.Add(new Field(nameof(AmphoraModel.IsPublic), DataType.Boolean)
            {
                IsFilterable = true
            });

            Name = IndexName;
        }
    }
}