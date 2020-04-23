using Amphora.Api.Options;
using Amphora.Common.Configuration;
using Amphora.Common.Models.GitHub;
using Amphora.Common.Options;
using Amphora.Infrastructure.Models.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amphora.Api
{
    public static class ApiConfiguration
    {
        public static void RegisterOptions(IConfiguration configuration, IServiceCollection services, bool isDevelopment)
        {
            CommonConfiguration.RegisterOptions(configuration, services, isDevelopment);

            services.Configure<SignalOptions>(configuration.GetSection("Signals"));
            services.Configure<SendGridOptions>(configuration.GetSection("SendGrid"));
            services.Configure<TsiOptions>(configuration.GetSection("Tsi"));
            services.Configure<ChatOptions>(configuration.GetSection("Chat"));
            services.Configure<FeedbackOptions>(configuration.GetSection("Feedback"));
            services.Configure<CreateOptions>(configuration.GetSection("Create"));
            services.Configure<AmphoraManagementOptions>(configuration.GetSection("AmphoraManagement"));

            services.Configure<GitHubConfiguration>(configuration.GetSection("GitHubOptions"));
        }
    }
}