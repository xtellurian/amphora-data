using System.Collections.Generic;
using Amphora.Common.Models.DataRequests;
using Microsoft.Azure.Search.Models;

namespace Amphora.Api.Models.AzureSearch
{
    public partial class UnifiedSearchIndex : Index
    {
        protected void DataRequests()
        {
            // Location
            Fields.Add(new Field(nameof(DataRequestModel.UserIdVotes), DataType.Collection(DataType.String))
            {
                IsRetrievable = true,
                IsFilterable = true
            });
        }
    }
}
