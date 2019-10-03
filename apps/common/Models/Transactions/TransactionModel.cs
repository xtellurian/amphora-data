using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Users;

namespace Amphora.Common.Models.Transactions
{
    public class TransactionModel : Entity
    {
        public TransactionModel()
        {
            // empty ctor for EF Core
        }
        public TransactionModel(ApplicationUser user, AmphoraModel amphora)
        {
            this.User = user;
            this.UserId = user.Id;
            this.Amphora = amphora;
            this.AmphoraId = amphora.Id;
            this.OrganisationId = user.OrganisationId;
            this.CreatedDate = System.DateTime.UtcNow;
            this.Price = amphora.Price.HasValue ? amphora.Price.Value : 0;
            this.ttl = 60 * 60 * 24 * 90; // 90 days
        }
        public string OrganisationId { get; set; }
        public double? Price { get; set; }
        // navigation
        public string AmphoraId { get; set; }
        public virtual AmphoraModel Amphora { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}