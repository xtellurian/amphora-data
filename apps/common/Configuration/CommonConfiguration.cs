using Amphora.Common.Configuration.Options;
using Amphora.Common.Models.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amphora.Common.Configuration
{
    public static class CommonConfiguration
    {
        public static void RegisterOptions(IConfiguration configuration, IServiceCollection services, bool isDevelopment)
        {
            services.Configure<ExternalServices>(configuration.GetSection("ExternalServices"));
            services.Configure<AzureEventGridTopicOptions>("AppTopic", configuration.GetSection("EventGrid").GetSection("AppTopic"));
            services.Configure<CosmosOptions>(configuration.GetSection("Cosmos"));
            services.Configure<EventOptions>(configuration.GetSection(nameof(EventOptions)));

            Amphora.Common.AmphoraHost.SetEnvironmentName(configuration.GetSection("Environment")["Stack"]);
        }
    }
}