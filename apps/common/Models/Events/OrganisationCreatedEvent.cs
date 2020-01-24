using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;

namespace Amphora.Common.Models.Events
{
    public class OrganisationCreatedEvent : EventBase, IEvent
    {
        public OrganisationCreatedEvent(OrganisationModel org)
        {
            Subject = org.Id;
            Data = new {
                Name = org.Name,
                About = org.About,
                Address = org.Address,
                Website = org.WebsiteUrl,
                CreatedByUserId = org.CreatedById,
                CreatedByUserName = org.CreatedBy?.UserName,
                CreatedByEmail = org.CreatedBy?.Email,
            };
        }

        public string EventType => "AmphoraData.Organisations.OrganisationCreated";

        public object Data { get; private set; }

        public string Subject { get; private set; }
    }
}