using Amphora.Common.Models.Organisations;
using Microsoft.Azure.Search.Models;

namespace Amphora.Api.Models.AzureSearch
{
    public partial class UnifiedSearchIndex : Index
    {
        public void OrganisationFields()
        {
            // add description
            Fields.Add(new Field(nameof(OrganisationModel.About), DataType.String)
            {
                IsSearchable = true
            });

            // created by
            Fields.Add(new Field(nameof(OrganisationModel.WebsiteUrl), DataType.String)
            {
                IsRetrievable = true,
                IsFilterable = true,
                IsSearchable = true
            });
        }
    }
}