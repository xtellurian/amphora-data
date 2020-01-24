using Amphora.Common.Contracts;
using Amphora.Common.Models.Purchases;

namespace Amphora.Common.Models.Events
{
    public class PurchaseEvent : EventBase, IEvent
    {
        public PurchaseEvent(PurchaseModel purchase)
        {
            Data = new
            {
                PurchaseId = purchase.Id,
                AmphoraId = purchase.AmphoraId,
                Price = purchase.Price,
                PurchasedByUserId = purchase.PurchasedByUserId,
                PurchasedByOrgId = purchase.PurchasedByOrganisationId
            };
            Subject = purchase.Id;
        }

        public string EventType => "AmphoraData.Purchases.NewPurchase";

        public object Data { get; private set; }

        public string Subject { get; private set; }
    }
}