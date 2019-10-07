using System.Collections.Generic;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Users;
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
            fields.Add(new Field(nameof(AmphoraModel.CreatedBy), DataType.String)
            {
                IsRetrievable = true,
                IsSortable = true,
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
            // Has Purchased
            fields.Add(new Field(nameof(AmphoraModel.Purchases),
                        DataType.Collection(DataType.Complex),
                        new List<Field>
                        {
                            new Field(nameof(ApplicationUser.Id), DataType.String)
                            {
                                IsFilterable = true,
                            },
                            new Field(nameof(ApplicationUser.OrganisationId), DataType.String),
                            new Field(nameof(ApplicationUser.UserName), DataType.String),
                        }));

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