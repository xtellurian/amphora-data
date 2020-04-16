using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Common.Models.Events
{
    public class AmphoraCreatedEvent : EventBase, IEvent
    {
        public AmphoraCreatedEvent(AmphoraModel amphora)
        {
            this.Data = new AmphoraCreatedEventData(amphora.Id,
                                                    amphora.OrganisationId,
                                                    amphora.Price,
                                                    amphora.CreatedBy?.UserName);

            this.Subject = $"/amphora/api/amphorae/{amphora.Id}";
        }

        public string EventType => "AmphoraData.Amphorae.NewAmphora";

        public IEventData Data { get; private set; }

        public override string Subject { get; set; }

        private class AmphoraCreatedEventData : IEventData
        {
            public AmphoraCreatedEventData(string amphoraId,
                                           string? organisationId,
                                           double? price,
                                           string? triggeredByUserName)
            {
                FriendlyName = $"Amphora({amphoraId}) created by {triggeredByUserName}";
                AmphoraId = amphoraId;
                OrganisationId = organisationId;
                Price = price;
                TriggeredByUserName = triggeredByUserName;
            }

            public string? FriendlyName { get; set; }
            public string? AmphoraId { get; set; }
            public string? OrganisationId { get; set; }
            public string? TriggeredByUserName { get; set; }
            public double? Price { get; set; }
        }
    }
}