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
            switch (component?.ToLower())
            {
                case "signals":
                    return options.CurrentValue?.IsSignalsEnabled ?? true;
                case "invoices":
                    return options.CurrentValue?.IsInvoicesEnabled ?? true;
                default:
                    return false;
            }
        }
    }
}