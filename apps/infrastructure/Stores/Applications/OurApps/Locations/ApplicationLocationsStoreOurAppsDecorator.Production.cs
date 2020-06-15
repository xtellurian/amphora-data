using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Applications;
using Amphora.Common.Models.Options;
using Amphora.Common.Models.Platform;
using Amphora.Common.Stores.EFCore;
using Amphora.Infrastructure.Stores.Applications.OurApps;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Identity.Stores.EFCore
{
    public class ApplicationLocationsStoreOurAppsDecoratorProduction : StoreDecorator<ApplicationLocationModel>
    {
        private readonly ExternalServices externalServices;
        private readonly EnvironmentInfo envInfo;
        private readonly OAuthClientSecret mvcClientSecret;
        private readonly ILogger<ApplicationLocationsStoreOurAppsDecoratorProduction> logger;

        public ApplicationLocationsStoreOurAppsDecoratorProduction(IEntityStore<ApplicationLocationModel> store,
                                                          ILogger<ApplicationLocationsStoreOurAppsDecoratorProduction> logger,
                                                          IOptions<ExternalServices> externalServices,
                                                          IOptions<EnvironmentInfo> envInfo,
                                                          IOptions<OAuthClientSecret> mvcClientSecret) : base(store)
        {
            this.externalServices = externalServices.Value;
            this.envInfo = envInfo.Value;
            this.mvcClientSecret = mvcClientSecret.Value;
            this.logger = logger;
            logger.LogInformation("Using Production Our Apps decorator.");
        }

        public override async Task<ApplicationLocationModel?> ReadAsync(string id)
        {
            var am = await base.ReadAsync(id);
            return am ?? OurAppsProduction.Get(envInfo, externalServices)
                .FirstOrDefault(_ => _.Locations.Any(l => l.Id == id))
                ?.Locations.FirstOrDefault(_ => _.Id == id);
        }

        public override async Task<IEnumerable<ApplicationLocationModel>> QueryAsync(Expression<Func<ApplicationLocationModel, bool>> where)
        {
            var res = await base.QueryAsync(where);
            var locations = OurAppsProduction.Get(envInfo, externalServices)
               .SelectMany(_ => _.Locations);
            var ours = locations.Where(_ => where.Compile()(_));
            return ours.Union(res);
        }
    }
}