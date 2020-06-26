using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Applications;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Amphora.Api.AspNet.Cors
{
    public class ConnectedCorsPolicyProvider : ICorsPolicyProvider
    {
        private readonly IEntityStore<ApplicationLocationModel> store;

        public ConnectedCorsPolicyProvider(IEntityStore<ApplicationLocationModel> store)
        {
            this.store = store;
        }

        public async Task<CorsPolicy> GetPolicyAsync(HttpContext context, string policyName)
        {
            var locations = await store.QueryAsync(_ => true);
            var builder = new CorsPolicyBuilder()
                .SetIsOriginAllowed(o => locations.Any(_ => o == _.Origin))
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();

            return builder.Build();
        }
    }
}