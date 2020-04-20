using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Options;
using Amphora.Common.Services.Azure;
using Amphora.Common.Services.Events;
using Amphora.Infrastructure.Services.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amphora.Infrastructure.Modules
{
    public static class EventsModule
    {
        public static void RegisterEventsModule(this IServiceCollection services,
                                                  IConfiguration configuration,
                                                  bool isProduction)
        {
            if (configuration.IsPersistentStores() || isProduction)
            {
                services.AddTransient<IEventPublisher, EventGridService>();
            }
            else
            {
                services.AddTransient<IEventPublisher, LoggingEventPublisher>();
            }

            services.AddTransient<IEventRoot, RootEventPublisher>();
            services.AddTransient<ITelemetry, AppInsightsTelemetry>();
        }
    }
}