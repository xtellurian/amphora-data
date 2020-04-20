using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Common.Models.Events
{
    public class AmphoraCreatedEvent : EventBase, IEvent
    {
        public AmphoraCreatedEvent(AmphoraModel amphora)
        {
            var data = new EventDataDictionary($"Amphora({amphora.Id}) created by {amphora.CreatedBy?.UserName}");
            data.Set("AmphoraId", amphora.Id);
            data.Set("OrganisationId", amphora.OrganisationId);
            data.Set("Price", amphora.Price?.ToString() ?? "0");
            data.SetTriggeredByUsername(amphora?.CreatedBy?.UserName);
            this.Data = data;
            this.Subject = $"/amphora/api/amphorae/{amphora?.Id}";
        }

        public string EventType => "AmphoraData.Amphorae.NewAmphora";

        public IEventData Data { get; private set; }

        public override string Subject { get; set; }
    }
}