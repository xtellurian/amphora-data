using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Users;

namespace Amphora.Common.Models.Purchases
{
    public class PurchaseModel : Entity
    {
        public PurchaseModel()
        {
            // empty ctor for EF Core
        }
        public PurchaseModel(ApplicationUser user, AmphoraModel amphora)
        {
            this.PurchasedByUser = user;
            this.PurchasedByUserId = user.Id;
            this.Amphora = amphora;
            this.AmphoraId = amphora.Id;
            this.PurchasedByOrganisationId = user.OrganisationId;
            this.CreatedDate = System.DateTime.UtcNow;
            this.Price = amphora.Price.HasValue ? amphora.Price.Value : 0;
        }
        public double? Price { get; set; }
        
        // navigation
        public string AmphoraId { get; set; }
        public virtual AmphoraModel Amphora { get; set; }
        public string PurchasedByUserId { get; set; }
        public virtual ApplicationUser PurchasedByUser { get; set; }
        public string PurchasedByOrganisationId { get; set; }
        public virtual OrganisationModel PurchasedByOrganisation { get; set; }

    }
}