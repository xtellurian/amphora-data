using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Applications;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.AspNet.Cors
{
    public class ConnectedCorsPolicyProvider : ICorsPolicyProvider
    {
        private readonly IEntityStore<ApplicationLocationModel> store;
        private readonly ILogger<ConnectedCorsPolicyProvider> logger;

        public ConnectedCorsPolicyProvider(IEntityStore<ApplicationLocationModel> store, ILogger<ConnectedCorsPolicyProvider> logger)
        {
            this.store = store;
            this.logger = logger;
        }

        public async Task<CorsPolicy> GetPolicyAsync(HttpContext context, string policyName)
        {
            IEnumerable<ApplicationLocationModel> locations;
            try
            {
                locations = await store.QueryAsync(_ => true);
            }
            catch (Exception ex)
            {
                logger.LogError($"Applications query failed. {ex}");
                locations = new List<ApplicationLocationModel>();
            }

            var builder = new CorsPolicyBuilder()
                .SetIsOriginAllowed(o => locations.Any(_ => o == _.Origin))
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();

            return builder.Build();
        }
    }
}