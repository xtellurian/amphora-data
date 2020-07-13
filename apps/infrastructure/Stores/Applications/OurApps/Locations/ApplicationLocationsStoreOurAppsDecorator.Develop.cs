using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Applications;
using Amphora.Common.Stores.EFCore;
using Amphora.Infrastructure.Stores.Applications.OurApps;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Amphora.Identity.Stores.EFCore
{
    public class ApplicationLocationsStoreOurAppsDecoratorDevelop : StoreDecorator<ApplicationLocationModel>
    {
        private readonly ILogger<ApplicationLocationsStoreOurAppsDecoratorDevelop> logger;

        public ApplicationLocationsStoreOurAppsDecoratorDevelop(IEntityStore<ApplicationLocationModel> store,
                                                       ILogger<ApplicationLocationsStoreOurAppsDecoratorDevelop> logger) : base(store)
        {
            this.logger = logger;
            logger.LogWarning("Using development decorator for Applications Models");
        }

        public override async Task<ApplicationLocationModel?> ReadAsync(string id)
        {
            var am = await base.ReadAsync(id);
            return am ?? OurAppsDevelop.Get()
                .FirstOrDefault(_ => _.Locations.Any(l => l.Id == id))
                ?.Locations.FirstOrDefault(_ => _.Id == id);
        }

        public override async Task<IEnumerable<ApplicationLocationModel>> QueryAsync(Expression<Func<ApplicationLocationModel, bool>> where, int skip, int take)
        {
            var res = await base.QueryAsync(where, skip, take);
            var locations = OurAppsDevelop.Get()
               .SelectMany(_ => _.Locations);
            var ours = locations.Where(_ => where.Compile()(_));
            return ours.Union(res);
        }
    }
}