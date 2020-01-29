using Amphora.Common.Contracts;
using Amphora.Common.Models.DataRequests;

namespace Amphora.Common.Models.Events
{
    public class DataRequestCreatedEvent : EventBase, IEvent
    {
        public DataRequestCreatedEvent(DataRequestModel dataRequest)
        {
            Subject = dataRequest.Id;
            Data = new DataRequestCreatedEventData(dataRequest.CreatedBy?.OrganisationId,
                                                   dataRequest.Id,
                                                   dataRequest.CreatedBy?.UserName,
                                                   dataRequest.Name,
                                                   dataRequest.Description);
        }

        public string EventType => "AmphoraData.DataRequests.DataRequestCreated";

        public IEventData Data { get; private set; }

        public string Subject { get; private set; }

        private class DataRequestCreatedEventData : IEventData
        {
            public DataRequestCreatedEventData(string? organisationId,
                                               string dataRequestId,
                                               string? triggeredByUserName,
                                               string name,
                                               string description)
            {
                OrganisationId = organisationId;
                DataRequestId = dataRequestId;
                TriggeredByUserName = triggeredByUserName;
                Name = name;
                Description = description;
            }

            public string? AmphoraId { get; set; }
            public string? OrganisationId { get; set; }
            public string? TriggeredByUserName { get; set; }
            public string DataRequestId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }
}