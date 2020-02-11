using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Events;
using Amphora.Common.Models.Purchases;

namespace Amphora.Api.Services.Purchases
{
    public class CommissionTrackingService : ICommissionTrackingService
    {
        private readonly IEntityStore<CommissionModel> store;
        private readonly IEventPublisher eventPublisher;

        public CommissionTrackingService(IEntityStore<CommissionModel> store, IEventPublisher eventPublisher)
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
            await eventPublisher.PublishEventAsync(new CommissionEarnedEvent(commission));
            return commission;
        }
    }
}