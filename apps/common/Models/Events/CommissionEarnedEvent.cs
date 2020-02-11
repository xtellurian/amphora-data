using Amphora.Common.Contracts;
using Amphora.Common.Models.Purchases;

namespace Amphora.Common.Models.Events
{
    public class CommissionEarnedEvent : EventBase, IEvent
    {
        public CommissionEarnedEvent(CommissionModel model)
        {
            this.Data = new CommissionEventData
            {
                AmphoraId = model.PurchaseModel?.AmphoraId,
                OrganisationId = model.FromOrganisationId,
                TriggeredByUserName = model.TriggeredByUsername
            };

            Subject = model.FromOrganisationName ?? model.FromOrganisationId ?? "Unknown Org?";
        }

        public string EventType => "AmphoraData.Purchases.EarnedCommission";

        public IEventData Data { get; private set; }

        public string Subject { get; private set; }

        private class CommissionEventData : IEventData
        {
            public string? AmphoraId { get; set; }
            public string? OrganisationId { get; set; }
            public string? TriggeredByUserName { get; set; }
        }
    }
}