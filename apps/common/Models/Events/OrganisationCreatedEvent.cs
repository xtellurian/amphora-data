using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;

namespace Amphora.Common.Models.Events
{
    public class OrganisationCreatedEvent : EventBase, IEvent
    {
        public OrganisationCreatedEvent(OrganisationModel org)
        {
            Subject = $"/amphora/api/organisations/{org.Id}";
            Data = new OrganisationCreatedEventData(org.Id, org.Name, org?.About, org?.Address, org?.WebsiteUrl);
        }

        public string EventType => "AmphoraData.Organisations.OrganisationCreated";

        public IEventData Data { get; private set; }

        public override string Subject { get; set; }
        private class OrganisationCreatedEventData : IEventData
        {
            public OrganisationCreatedEventData(string? organisationId, string name, string? about, string? address, string? website)
            {
                FriendlyName = $"Organisation({organisationId}) was created";
                OrganisationId = organisationId;
                Name = name;
                About = about;
                Address = address;
                Website = website;
            }

            public string? FriendlyName { get; set; }
            public string? AmphoraId { get; set; }
            public string? OrganisationId { get; set; }
            public string? TriggeredByUserName { get; set; }
            public string Name { get; set; }
            public string? About { get; set; }
            public string? Address { get; set; }
            public string? Website { get; set; }
        }
    }
}