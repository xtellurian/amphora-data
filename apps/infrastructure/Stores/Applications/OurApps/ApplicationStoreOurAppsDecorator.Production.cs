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
    public class ApplicationStoreOurAppsDecoratorProduction : StoreDecorator<ApplicationModel>
    {
        private readonly ExternalServices externalServices;
        private readonly EnvironmentInfo envInfo;
        private readonly OAuthClientSecret mvcClientSecret;
        private readonly ILogger<ApplicationStoreOurAppsDecoratorProduction> logger;

        public ApplicationStoreOurAppsDecoratorProduction(IEntityStore<ApplicationModel> store,
                                                          ILogger<ApplicationStoreOurAppsDecoratorProduction> logger,
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

        public override async Task<ApplicationModel?> ReadAsync(string id)
        {
            var am = await base.ReadAsync(id);
            return am ?? OurAppsProduction.Get(envInfo, externalServices).FirstOrDefault(_ => _.Id == id);
        }

        public override async Task<IEnumerable<ApplicationModel>> QueryAsync(Expression<Func<ApplicationModel, bool>> where, int skip, int take)
        {
            var res = await base.QueryAsync(where, skip, take);
            var ours = OurAppsProduction.Get(envInfo, externalServices).Where(_ => where.Compile()(_));
            return ours.Union(res);
        }
    }
}