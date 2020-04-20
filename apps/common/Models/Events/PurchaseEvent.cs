using System.Collections.Generic;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Purchases;

namespace Amphora.Common.Models.Events
{
    public class PurchaseEvent : EventBase, IEvent
    {
        public PurchaseEvent(PurchaseModel purchase)
        {
            var d = new EventDataDictionary($"Amphora({purchase.AmphoraId}) purchased by User({purchase.PurchasedByUser?.UserName})");
            d.SetOrganisationId(purchase.PurchasedByOrganisationId);
            d.SetAmphoraId(purchase.AmphoraId);
            d.Set("Price", purchase.Price?.ToString());
            this.Data = d;

            Subject = $"/amphora/api/amphorae/{purchase.AmphoraId}/purchases";
        }

        public string EventType => "AmphoraData.Purchases.NewPurchase";
        public IEventData Data { get; private set; }
        public override string Subject { get; set; }
    }
}