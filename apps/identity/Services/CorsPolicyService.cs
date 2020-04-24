using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Services;

namespace Amphora.Identity.Services
{
    public class CorsPolicyService : ICorsPolicyService
    {
        private List<string> allowedOrigins = new List<string>
        {
            "https://localhost:5001",
            "http://localhost:5000"
        };

        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            return Task.FromResult(allowedOrigins.Contains(origin));
        }
    }
}