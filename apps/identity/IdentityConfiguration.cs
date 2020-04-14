using Amphora.Common.Configuration;
using Amphora.Infrastructure.Models.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amphora.Identity
{
    public static class IdentityConfiguration
    {
        public static void RegisterOptions(IConfiguration configuration, IServiceCollection services)
        {
            CommonConfiguration.RegisterOptions(configuration, services);
            services.Configure<SendGridOptions>(configuration.GetSection("SendGrid"));
        }
    }
}