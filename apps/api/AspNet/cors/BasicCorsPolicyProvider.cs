using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Amphora.Api.AspNet.Cors
{
    public class BasicCorsPolicyProvider : ICorsPolicyProvider
    {
        private List<string> origins = new List<string>
        {
            "http://localhost:3000"
        };

        private readonly IEntityStore<AmphoraModel> store;

        public BasicCorsPolicyProvider(IEntityStore<AmphoraModel> store)
        {
            this.store = store;
        }

        public Task<CorsPolicy> GetPolicyAsync(HttpContext context, string policyName)
        {
            var builder = new CorsPolicyBuilder()
                .SetIsOriginAllowed(o => origins.Contains(o))
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();

            return Task.FromResult(builder.Build());
        }
    }
}