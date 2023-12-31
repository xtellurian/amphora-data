using System;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Users;

namespace Amphora.Common.Models.Purchases
{
    public class PurchaseModel : EntityBase
    {
        public PurchaseModel()
        {
            AmphoraId = null!;
            PurchasedByUserId = null!;
            PurchasedByOrganisationId = null!;
        }

        protected PurchaseModel(string amphoraId, string purchasedByUserId, string purchasedByOrganisationId)
        {
            AmphoraId = amphoraId;
            PurchasedByUserId = purchasedByUserId;
            PurchasedByOrganisationId = purchasedByOrganisationId;
        }

        public PurchaseModel(ApplicationUserDataModel user, AmphoraModel amphora, DateTimeOffset? timestamp)
        {
            if (user.OrganisationId == null)
            {
                throw new NullReferenceException($"User OrganisationId was null, userId: {user.OrganisationId}");
            }

            this.PurchasedByUser = user;
            this.PurchasedByUserId = user.Id;
            this.Amphora = amphora;
            this.AmphoraId = amphora.Id;
            this.PurchasedByOrganisationId = user.OrganisationId;
            this.CreatedDate = timestamp ?? System.DateTimeOffset.UtcNow;
            this.LastModified = timestamp ?? System.DateTimeOffset.UtcNow;
            this.Price = amphora.Price ?? 0;
        }

        public double? Price { get; set; }
        public DateTimeOffset? LastDebitTime { get; set; }

        // navigation
        public string AmphoraId { get; set; }
        public virtual AmphoraModel Amphora { get; set; } = null!;
        public string? PurchasedByUserId { get; set; }
        public virtual ApplicationUserDataModel? PurchasedByUser { get; set; } = null!;
        public string? PurchasedByOrganisationId { get; set; }
        public virtual OrganisationModel? PurchasedByOrganisation { get; set; } = null!;
    }
}