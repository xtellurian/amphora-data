using System.Collections.Generic;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Purchases;

namespace Amphora.Common.Models.Events
{
    public class CommissionEarnedEvent : EventBase, IEvent
    {
        public CommissionEarnedEvent(CommissionModel model)
        {
            var data = new EventDataDictionary($"Commission Earned from Amphora({model.PurchaseModel?.AmphoraId})");
            data.SetAmphoraId(model.PurchaseModel?.AmphoraId);
            data.SetOrganisationId(model.FromOrganisationId);
            data.SetTriggeredByUsername(model.PurchaseModel?.PurchasedByUser?.UserName);
            this.Data = data;

            Subject = $"/amphora/api/admin/amphorae/{model.PurchaseModel?.AmphoraId}";
        }

        public string EventType => "AmphoraData.Purchases.EarnedCommission";

        public IEventData Data { get; private set; }

        public override string Subject { get; set; }
    }
}