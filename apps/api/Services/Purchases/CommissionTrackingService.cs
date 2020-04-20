using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Events;
using Amphora.Common.Models.Purchases;

namespace Amphora.Api.Services.Purchases
{
    public class CommissionTrackingService : ICommissionTrackingService
    {
        private readonly IEntityStore<CommissionModel> store;
        private readonly IEventRoot eventPublisher;

        public CommissionTrackingService(IEntityStore<CommissionModel> store, IEventRoot eventPublisher)
        {
            this.store = store;
            this.eventPublisher = eventPublisher;
        }

        public async Task<CommissionModel> TrackCommissionAsync(PurchaseModel purchase, double? commissionAmount)
        {
            var commission = new CommissionModel
            {
                Amount = commissionAmount,
                FromOrganisationId = purchase.Amphora.OrganisationId,
                FromOrganisationName = purchase.Amphora.Organisation.Name,
                TriggeredByUsername = purchase.PurchasedByUser.UserName,
                PurchaseModelId = purchase.Id,
                PurchaseModel = purchase
            };
            commission = await store.CreateAsync(commission);
            if (commissionAmount > 0)
            {
                await eventPublisher.PublishEventAsync(new CommissionEarnedEvent(commission));
            }

            return commission;
        }
    }
}