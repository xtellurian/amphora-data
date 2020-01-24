using Amphora.Common.Contracts;
using Amphora.Common.Models.DataRequests;

namespace Amphora.Common.Models.Events
{
    public class DataRequestCreatedEvent : EventBase, IEvent
    {
        public DataRequestCreatedEvent(DataRequestModel dataRequest)
        {
            Subject = dataRequest.Id;
            Data = new
            {
                DataRequestId = dataRequest.Id,
                Name = dataRequest.Name,
                Description = dataRequest.Description,
                CreatedByUserId = dataRequest.CreatedById,
                CreatedByUserName = dataRequest.CreatedBy?.UserName,
                CreatedByUserEmail = dataRequest.CreatedBy?.Email,
                CreatedByUserOrg = dataRequest.CreatedBy?.OrganisationId
            };
        }

        public string EventType => "AmphoraData.DataRequests.DataRequestCreated";

        public object Data { get; private set; }

        public string Subject { get; private set; }
    }
}