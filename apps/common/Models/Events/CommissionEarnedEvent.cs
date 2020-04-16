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
                FriendlyName = $"Commission Earned from Amphora({model.PurchaseModel?.AmphoraId})",
                AmphoraId = model.PurchaseModel?.AmphoraId,
                OrganisationId = model.FromOrganisationId,
                TriggeredByUserName = model.TriggeredByUsername
            };

            Subject = $"/amphora/api/admin/amphorae/{model.PurchaseModel?.AmphoraId}";
        }

        public string EventType => "AmphoraData.Purchases.EarnedCommission";

        public IEventData Data { get; private set; }

        public override string Subject { get; set; }

        private class CommissionEventData : IEventData
        {
            public string? FriendlyName { get; set; }
            public string? AmphoraId { get; set; }
            public string? OrganisationId { get; set; }
            public string? TriggeredByUserName { get; set; }
        }
    }
}