using Amphora.Common.Contracts;
using Amphora.Common.Models.Purchases;

namespace Amphora.Common.Models.Events
{
    public class PurchaseEvent : EventBase, IEvent
    {
        public PurchaseEvent(PurchaseModel purchase)
        {
            Data = new PurchaseEventData(purchase.AmphoraId,
                                         purchase.PurchasedByOrganisationId,
                                         purchase.PurchasedByUser?.UserName,
                                         purchase.Price);

            Subject = $"/amphora/api/amphorae/{purchase.AmphoraId}/purchases";
        }

        public string EventType => "AmphoraData.Purchases.NewPurchase";

        public IEventData Data { get; private set; }

        public override string Subject { get; set; }

        private class PurchaseEventData : IEventData
        {
            public PurchaseEventData(string? amphoraId, string? organisationId, string? triggeredByUserName, double? price)
            {
                FriendlyName = $"Amphora({amphoraId}) purchased by User({triggeredByUserName})";
                AmphoraId = amphoraId;
                OrganisationId = organisationId;
                TriggeredByUserName = triggeredByUserName;
                Price = price;
            }

            public string? FriendlyName { get; set; }
            public string? AmphoraId { get; set; }
            public string? OrganisationId { get; set; }
            public string? TriggeredByUserName { get; set; }
            public double? Price { get; set; }
        }
    }
}