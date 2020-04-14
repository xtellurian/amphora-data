using Amphora.Common.Configuration.Options;
using Amphora.Common.Models.Host;
using Amphora.Common.Models.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amphora.Common.Configuration
{
    public static class CommonConfiguration
    {
        public static void RegisterOptions(IConfiguration configuration, IServiceCollection services)
        {
            services.Configure<ExternalServices>(configuration.GetSection("ExternalServices"));
            services.Configure<AzureEventGridTopicOptions>("AppTopic", configuration.GetSection("EventGrid").GetSection("AppTopic"));
            services.Configure<HostOptions>(configuration.GetSection("Host"));
            services.Configure<CosmosOptions>(configuration.GetSection("Cosmos"));
        }
    }
}