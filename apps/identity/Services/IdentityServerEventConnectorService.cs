using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Events;
using Amphora.Common.Models.Options;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.Extensions.Options;

namespace Amphora.Identity.Services
{
    public class IdentityServerEventConnectorService : IEventSink
    {
        private readonly IEventPublisher eventPublisher;
        private readonly EventOptions options;

        public IdentityServerEventConnectorService(IOptionsMonitor<EventOptions> options, IEventPublisher eventPublisher)
        {
            this.eventPublisher = eventPublisher;
            this.options = options.CurrentValue;
        }

        public async Task PersistAsync(Event evt)
        {
            if (options?.IdentityServer == true)
            {
                var e = new IdentityServerEvent(evt.Name, evt.Message, evt.Category);
                await eventPublisher.PublishEventAsync(e);
            }
        }
    }
}