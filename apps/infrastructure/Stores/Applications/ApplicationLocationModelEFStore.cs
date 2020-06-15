using Amphora.Common.Contracts;
using Amphora.Common.Models.Applications;
using Amphora.Common.Stores.EFCore;
using Amphora.Infrastructure.Database.Contexts;
using Microsoft.Extensions.Logging;

namespace Amphora.Infrastructure.Stores.Applications
{
    public class ApplicationLocationModelEFStore : EFStoreBase<ApplicationLocationModel, ApplicationsContext>, IEntityStore<ApplicationLocationModel>
    {
        public ApplicationLocationModelEFStore(ApplicationsContext context, ILogger<ApplicationLocationModelEFStore> logger)
            : base(context, logger, s => s.Locations)
        {
        }
    }
}