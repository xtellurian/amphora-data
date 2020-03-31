using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations.Accounts;

namespace Amphora.Common.Models.Events
{
    public class PlanUpdatedEvent : EventBase, IEvent
    {
        public PlanUpdatedEvent(string triggeredByUserName, Account account)
        {
            Subject = $"organisation|{account.OrganisationId}";
            Data = new PlanUpdatedData
            {
                OrganisationId = account.OrganisationId,
                TriggeredByUserName = triggeredByUserName,
                NewPlanName = account?.Plan?.PlanType.ToString()
            };
        }

        public string EventType => "AmphoraData.Organisations.PlanUpdated";

        public IEventData Data { get; }

        public string? Subject { get; }

        private class PlanUpdatedData : IEventData
        {
            public string? AmphoraId { get; set; }
            public string? OrganisationId { get; set; }
            public string? TriggeredByUserName { get; set; }
            public string? NewPlanName { get; set; }
        }
    }
}