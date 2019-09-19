using System.Collections.Generic;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.UserData;
using Microsoft.Azure.Search.Models;

namespace Amphora.Api.Models.AzureSearch
{
    public class AmphoraSearchIndex : Index
    {
        public AmphoraSearchIndex()
        {
            var fields = new List<Field>();
            fields.Add(new Field(nameof(AmphoraModel.Id), DataType.String));
            fields.Add(new Field(nameof(AmphoraModel.AmphoraId), DataType.String)
            {
                IsKey = true // key of AmphoraId because Id has a special charcter not allowed
            });
            // org id for filtering 
            fields.Add(new Field(nameof(Entity.OrganisationId), DataType.String)
            {
                IsFacetable = true,
                IsRetrievable = true,
                IsFilterable = true
            });
            // type
            fields.Add(new Field(nameof(Entity.EntityType), DataType.String)
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
            fields.Add(new Field(nameof(AmphoraExtendedModel.Description), DataType.String)
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
            fields.Add(new Field(nameof(AmphoraExtendedModel.GeoLocation), DataType.GeographyPoint)
            {
                IsRetrievable = true,
                IsSortable = true,
                IsFilterable = true
            });
            // Has Purchased
            fields.Add(new Field(nameof(AmphoraSecurityModel.HasPurchased),
                        DataType.Collection(DataType.Complex),
                        new List<Field>
                        {
                            new Field(nameof(ApplicationUserReference.Id), DataType.String)
                            {
                                IsFilterable = true,
                            },
                            new Field(nameof(ApplicationUserReference.OrganisationId), DataType.String),
                            new Field(nameof(ApplicationUserReference.UserName), DataType.String),
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