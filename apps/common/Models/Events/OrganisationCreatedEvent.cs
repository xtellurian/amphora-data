using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;

namespace Amphora.Common.Models.Events
{
    public class OrganisationCreatedEvent : EventBase, IEvent
    {
        public OrganisationCreatedEvent(OrganisationModel org)
        {
            Subject = $"/amphora/api/organisations/{org.Id}";
            var data = new EventDataDictionary($"Organisation({org.Id}) was created");
            data.SetOrganisationId(org.Id);
            data.SetTriggeredByUsername(org.CreatedBy?.UserName);
            data.Set("Name", org.Name);
            data.Set("About", org.About);
            data.Set("Website", org.WebsiteUrl);
            data.Set("Address", org.Address);
            this.Data = data;
        }

        public string EventType => "AmphoraData.Organisations.OrganisationCreated";

        public IEventData Data { get; private set; }

        public override string Subject { get; set; }
    }
}