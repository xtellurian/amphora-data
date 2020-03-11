using Amphora.Api.Contracts;
using Amphora.Api.EntityFramework;
using Amphora.Common.Contracts;
using Amphora.Common.Models.DataRequests;
using Amphora.Common.Stores.EFCore;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public class DataRequestsEFStore : EFStoreBase<DataRequestModel, AmphoraContext>, IEntityStore<DataRequestModel>
    {
        public DataRequestsEFStore(AmphoraContext context, ILogger<DataRequestsEFStore> logger) : base(context, logger, db => db.DataRequests)
        {
        }
    }
}