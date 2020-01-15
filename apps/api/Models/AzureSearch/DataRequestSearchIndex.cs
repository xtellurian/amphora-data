using System.Collections.Generic;
using Amphora.Common.Models.DataRequests;
using Microsoft.Azure.Search.Models;

namespace Amphora.Api.Models.AzureSearch
{
    public class DataRequestSearchIndex : Index
    {
        public DataRequestSearchIndex() : base()
        {
            var fields = new List<Field>();
            fields.Add(new Field(nameof(DataRequestModel.Id), DataType.String)
            {
                IsKey = true
            });
            // type
            fields.Add(new Field("Discriminator", DataType.String)
            {
                IsFilterable = true
            });
            // add name
            fields.Add(new Field(nameof(DataRequestModel.Name), DataType.String)
            {
                IsSearchable = true,
                IsRetrievable = true
            });
            // add description
            fields.Add(new Field(nameof(DataRequestModel.Description), DataType.String)
            {
                IsSearchable = true
            });
            // add IsDeleted for soft delete
            fields.Add(new Field(nameof(DataRequestModel.IsDeleted), DataType.Boolean)
            {
                IsFilterable = true
            });
            // created by
            fields.Add(new Field(nameof(DataRequestModel.CreatedById), DataType.String)
            {
                IsRetrievable = true,
                IsFacetable = true,
                IsFilterable = true
            });
            // Location
            fields.Add(new Field(nameof(DataRequestModel.GeoLocation), DataType.GeographyPoint)
            {
                IsRetrievable = true,
                IsSortable = true,
                IsFilterable = true
            });
            // Location
            fields.Add(new Field(nameof(DataRequestModel.UserIdVotes), DataType.Collection(DataType.String))
            {
                IsRetrievable = true,
                IsFilterable = true
            });

            Name = IndexName;
            Fields = fields;
        }

        public static string IndexName => $"{ApiVersion.CurrentVersion.ToSemver('-')}-datarequest-index";
    }
}