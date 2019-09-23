using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.UserData;

namespace Amphora.Common.Models.Transactions
{
    public class TransactionModel : Entity
    {
        public TransactionModel()
        {
            this.EntityType = typeof(TransactionModel).GetEntityPrefix();
        }
        public TransactionModel(IApplicationUserReference user, AmphoraModel amphora): this()
        {
            this.User = new ApplicationUserReference
            {
                Id = user.Id,
                UserName = user.UserName,
                OrganisationId = user.OrganisationId
            };

            this.AmphoraId = amphora.AmphoraId;
            this.CreatedBy = user.Id;
            this.CreatedDate = System.DateTime.UtcNow;
            this.OrganisationId = user.OrganisationId;
            this.Price = amphora.Price.HasValue ? amphora.Price.Value : 0;
            this.ttl = 60 * 60 * 24 * 90; // 90 days
        }
        public string TransactionId { get; set; }
        public string AmphoraId { get; set; }
        public ApplicationUserReference User {get; set; }
        public double? Price { get; set; }
        public override void SetIds()
        {
            this.TransactionId = System.Guid.NewGuid().ToString();
            this.Id = this.TransactionId.AsQualifiedId<TransactionModel>();
        }
    }
}