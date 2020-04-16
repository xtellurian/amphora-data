using Amphora.Common.Contracts;
using Amphora.Common.Models.DataRequests;

namespace Amphora.Common.Models.Events
{
    public class DataRequestCreatedEvent : EventBase, IEvent
    {
        public DataRequestCreatedEvent(DataRequestModel dataRequest)
        {
            Subject = $"/amphora/api/datarequests/{dataRequest.Id}";
            Data = new DataRequestCreatedEventData(dataRequest.CreatedBy?.OrganisationId,
                                                   dataRequest.Id,
                                                   dataRequest.CreatedBy?.UserName,
                                                   dataRequest.Name,
                                                   dataRequest.Description);
        }

        public string EventType => "AmphoraData.DataRequests.DataRequestCreated";

        public IEventData Data { get; private set; }

        public override string Subject { get; set; }

        private class DataRequestCreatedEventData : IEventData
        {
            public DataRequestCreatedEventData(string? organisationId,
                                               string dataRequestId,
                                               string? triggeredByUserName,
                                               string name,
                                               string description)
            {
                FriendlyName = $"DataRequest({dataRequestId}) created";
                OrganisationId = organisationId;
                DataRequestId = dataRequestId;
                TriggeredByUserName = triggeredByUserName;
                Name = name;
                Description = description;
            }

            public string? FriendlyName { get; set; }
            public string? AmphoraId { get; set; }
            public string? OrganisationId { get; set; }
            public string? TriggeredByUserName { get; set; }
            public string DataRequestId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }
}