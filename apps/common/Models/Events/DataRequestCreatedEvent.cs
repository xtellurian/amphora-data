using System.Collections.Generic;
using Amphora.Common.Contracts;
using Amphora.Common.Models.DataRequests;

namespace Amphora.Common.Models.Events
{
    public class DataRequestCreatedEvent : EventBase, IEvent
    {
        public DataRequestCreatedEvent(DataRequestModel dataRequest)
        {
            Subject = $"/amphora/api/datarequests/{dataRequest.Id}";
            var data = new EventDataDictionary($"DataRequest({dataRequest.Id}) created");
            data.SetOrganisationId(dataRequest.CreatedBy?.OrganisationId);
            data.SetTriggeredByUsername(dataRequest.CreatedBy?.UserName);
            data.Set("Name", dataRequest.Name);
            data.Set("Description", dataRequest.Description);
            this.Data = data;
        }

        public string EventType => "AmphoraData.DataRequests.DataRequestCreated";

        public IEventData Data { get; private set; }

        public override string Subject { get; set; }
    }
}