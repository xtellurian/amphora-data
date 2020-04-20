using System.Collections.Generic;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations.Accounts;

namespace Amphora.Common.Models.Events
{
    public class PlanUpdatedEvent : EventBase, IEvent
    {
        public PlanUpdatedEvent(string triggeredByUserName, Account account)
        {
            Subject = $"/amphora/api/organisations/{account.OrganisationId}/account";
            var data = new EventDataDictionary($"Plan updated for Organsation({account.OrganisationId}) to {account?.Plan?.PlanType} Plan");
            data.SetOrganisationId(account?.OrganisationId);
            data.SetTriggeredByUsername(triggeredByUserName);
            data.Set("NewPlanType", account?.Plan?.PlanType.ToString());
            this.Data = data;
        }

        public string EventType => "AmphoraData.Organisations.PlanUpdated";
        public IEventData Data { get; }
        public override string Subject { get; set; }
    }
}