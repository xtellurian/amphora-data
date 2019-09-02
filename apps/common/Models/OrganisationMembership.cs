using Amphora.Common.Extensions;

namespace Amphora.Common.Models
{
    public class OrganisationMembership : Entity
    {
        public string OrganisationMembershipId { get; set; }
        public string Role { get; set; }
        public string UserId { get; set; }

        public override void SetIds()
        {
            this.OrganisationMembershipId = System.Guid.NewGuid().ToString();
            this.Id = this.OrganisationMembershipId.AsQualifiedId(typeof(OrganisationMembership));
        }
    }
}