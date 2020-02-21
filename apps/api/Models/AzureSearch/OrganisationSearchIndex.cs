using System.Collections.Generic;
using Amphora.Common.Models.Organisations;
using Microsoft.Azure.Search.Models;

namespace Amphora.Api.Models.AzureSearch
{
    public class OrganisationSearchIndex : Index
    {
        public OrganisationSearchIndex() : base()
        {
            var fields = new List<Field>();
            fields.Add(new Field(nameof(OrganisationModel.Id), DataType.String)
            {
                IsKey = true
            });
            // type
            fields.Add(new Field("Discriminator", DataType.String)
            {
                IsFilterable = true
            });
            // add name
            fields.Add(new Field(nameof(OrganisationModel.Name), DataType.String)
            {
                IsSearchable = true,
                IsRetrievable = true
            });
            // add description
            fields.Add(new Field(nameof(OrganisationModel.About), DataType.String)
            {
                IsSearchable = true
            });
            // add IsDeleted for soft delete
            fields.Add(new Field(nameof(OrganisationModel.IsDeleted), DataType.Boolean)
            {
                IsFilterable = true
            });
            // created by
            fields.Add(new Field(nameof(OrganisationModel.WebsiteUrl), DataType.String)
            {
                IsRetrievable = true,
                IsFilterable = true,
                IsSearchable = true
            });

            Name = IndexName;
            Fields = fields;
        }

        public static string IndexName => $"{ApiVersion.CurrentVersion.ToSemver('-')}-organisation-index";
    }
}