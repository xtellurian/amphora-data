using Amphora.Api.Options;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Services.FeatureFlags
{
    public class FeatureFlagService
    {
        private readonly IOptionsMonitor<FeatureFlagOptions> options;

        public FeatureFlagService(IOptionsMonitor<FeatureFlagOptions> options)
        {
            this.options = options;
        }

        public bool IsEnabled(string component)
        {
            if(string.Equals(component, "signals"))
            {
                return options.CurrentValue?.IsSignalsEnabled ?? true;
            }
            else
            {
                return true;
            }
        }
    }
}