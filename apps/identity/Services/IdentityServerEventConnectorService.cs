using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Events;
using IdentityServer4.Events;
using IdentityServer4.Services;

namespace Amphora.Identity.Services
{
    public class IdentityServerEventConnectorService : IEventSink
    {
        private readonly IEventPublisher eventPublisher;

        public IdentityServerEventConnectorService(IEventPublisher eventPublisher)
        {
            this.eventPublisher = eventPublisher;
        }

        public async Task PersistAsync(Event evt)
        {
            var e = new IdentityServerEvent(evt.Name, evt.Message, evt.Category);
            await eventPublisher.PublishEventAsync(e);
        }
    }
}