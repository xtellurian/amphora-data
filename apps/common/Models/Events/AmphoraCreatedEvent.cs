using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Common.Models.Events
{
    public class AmphoraCreatedEvent : EventBase, IEvent
    {
        public AmphoraCreatedEvent(AmphoraModel amphora)
        {
            this.Data = new
            {
                AmphoraId = amphora.Id,
                Price = amphora.Price,
                CreatedByUserId = amphora.CreatedById,
                CreatedByUserName = amphora.CreatedBy?.UserName,
                OrganisationId = amphora.OrganisationId
            };

            this.Subject = amphora.Id;
        }

        public string EventType => "AmphoraData.Amphora.NewAmphora";

        public object Data { get; private set; }

        public string Subject { get; private set; }
    }
}