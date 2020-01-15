using Amphora.Api.Contracts;
using Amphora.Api.EntityFramework;
using Amphora.Common.Models.DataRequests;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public class DataRequestsEFStore : EFStoreBase<DataRequestModel>, IEntityStore<DataRequestModel>
    {
        public DataRequestsEFStore(AmphoraContext context, ILogger<DataRequestsEFStore> logger) : base(context, logger, db => db.DataRequests)
        {
        }
    }
}