using Amphora.Common.Contracts;
using Amphora.Common.Models.Applications;
using Amphora.Common.Stores.EFCore;
using Amphora.Infrastructure.Database.Contexts;
using Microsoft.Extensions.Logging;

namespace Amphora.Infrastructure.Stores.Applications
{
    public class ApplicationModelEFStore : EFStoreBase<ApplicationModel, ApplicationsContext>, IEntityStore<ApplicationModel>
    {
        public ApplicationModelEFStore(ApplicationsContext context, ILogger<ApplicationModelEFStore> logger)
            : base(context, logger, s => s.Applications)
        {
        }
    }
}