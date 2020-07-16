using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Applications;
using Amphora.Common.Stores.EFCore;
using Amphora.Infrastructure.Stores.Applications.OurApps;
using Microsoft.Extensions.Logging;

namespace Amphora.Identity.Stores.EFCore
{
    public class ApplicationStoreOurAppsDecoratorDevelop : StoreDecorator<ApplicationModel>
    {
        private readonly ILogger<ApplicationStoreOurAppsDecoratorDevelop> logger;

        public ApplicationStoreOurAppsDecoratorDevelop(IEntityStore<ApplicationModel> store,
                                                       ILogger<ApplicationStoreOurAppsDecoratorDevelop> logger) : base(store)
        {
            this.logger = logger;
            logger.LogWarning("Using development decorator for Applications Models");
        }

        public override async Task<ApplicationModel?> ReadAsync(string id)
        {
            var am = await base.ReadAsync(id);
            return am ?? OurAppsDevelop.Get().FirstOrDefault(_ => _.Id == id);
        }

        public override async Task<IEnumerable<ApplicationModel>> QueryAsync(Expression<Func<ApplicationModel, bool>> where, int skip, int take)
        {
            var res = await base.QueryAsync(where, skip, take);
            var ours = OurAppsDevelop.Get().Where(_ => where.Compile()(_));
            return ours.Union(res);
        }
    }
}