using System.Collections.Generic;
using Amphora.Common.Models.Amphorae;
using Microsoft.Azure.Search.Models;

namespace Amphora.Api.Models.AzureSearch
{
    public class AmphoraSearchIndex : Index
    {
        public AmphoraSearchIndex(): base( )
        {
            var fields = new List<Field>();
            fields.Add(new Field(nameof(AmphoraModel.Id), DataType.String)
            {
                IsKey = true
            });
            // org id for filtering 
            fields.Add(new Field(nameof(AmphoraModel.OrganisationId), DataType.String)
            {
                IsFacetable = true,
                IsRetrievable = true,
                IsFilterable = true
            });
            // type
            fields.Add(new Field("Discriminator", DataType.String)
            {
                IsFilterable = true
            });
            // add name
            fields.Add(new Field(nameof(AmphoraModel.Name), DataType.String)
            {
                IsSearchable = true,
                IsRetrievable = true
            });
            // add about
            fields.Add(new Field(nameof(AmphoraModel.Description), DataType.String)
            {
                IsSearchable = true
            });
            // add price
            fields.Add(new Field(nameof(AmphoraModel.Price), DataType.Double)
            {
                IsRetrievable = true,
                IsSortable = true,
                IsFacetable = true,
                IsFilterable = true
            });
            // created by
            fields.Add(new Field(nameof(AmphoraModel.CreatedById), DataType.String)
            {
                IsRetrievable = true,
                IsFacetable = true,
                IsFilterable = true
            });
            // Location
            fields.Add(new Field(nameof(AmphoraModel.GeoLocation), DataType.GeographyPoint)
            {
                IsRetrievable = true,
                IsSortable = true,
                IsFilterable = true
            });

            // add isPubic
            fields.Add(new Field(nameof(AmphoraModel.IsPublic), DataType.Boolean)
            {
                IsFilterable = true
            });

            Name = IndexName;
            Fields = fields;
        }

        public static string IndexName => "amphora-index";
    }
}