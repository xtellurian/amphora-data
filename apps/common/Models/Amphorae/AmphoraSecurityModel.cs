using System.Collections.Generic;
using System.Linq;
using Amphora.Common.Contracts;
using Amphora.Common.Models.UserData;

namespace Amphora.Common.Models.Amphorae
{
    public class AmphoraSecurityModel : AmphoraModel
    {
        public List<ApplicationUserReference> HasPurchased { get; set; }

        public void AddUserHasPurchased(IApplicationUserReference user)
        {
            if(this.HasPurchased == null) this.HasPurchased = new List<ApplicationUserReference>();
            if(this.HasPurchased.Any(u => string.Equals(u.Id, user.Id))) return;
            this.HasPurchased.Add(new ApplicationUserReference()
            {
                Id = user.Id,
                OrganisationId = user.OrganisationId,
                UserName = user.UserName
            });
        }
    }
}