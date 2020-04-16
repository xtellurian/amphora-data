using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations.Accounts;

namespace Amphora.Common.Models.Events
{
    public class PlanUpdatedEvent : EventBase, IEvent
    {
        public PlanUpdatedEvent(string triggeredByUserName, Account account)
        {
            Subject = $"/amphora/api/organisations/{account.OrganisationId}/account";
            Data = new PlanUpdatedData
            {
                FriendlyName = $"Plan updated for Organsation({account.OrganisationId}) to {account?.Plan?.PlanType} Plan",
                OrganisationId = account?.OrganisationId,
                TriggeredByUserName = triggeredByUserName,
                NewPlanName = account?.Plan?.PlanType.ToString()
            };
        }

        public string EventType => "AmphoraData.Organisations.PlanUpdated";

        public IEventData Data { get; }

        public override string Subject { get; set; }

        private class PlanUpdatedData : IEventData
        {
            public string? FriendlyName { get; set; }
            public string? AmphoraId { get; set; }
            public string? OrganisationId { get; set; }
            public string? TriggeredByUserName { get; set; }
            public string? NewPlanName { get; set; }
        }
    }
}