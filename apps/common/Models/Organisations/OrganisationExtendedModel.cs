using System.Collections.Generic;
using System.Linq;
using Amphora.Common.Contracts;

namespace Amphora.Common.Models.Organisations
{
    public class OrganisationExtendedModel : OrganisationModel
    {
        public List<Invitation> Invitations { get; set; }
        public List<Membership> Memberships { get; set; }
        public string About { get; set; }
        public string WebsiteUrl { get; set; }
        public string Address { get; set; }
        public void AddInvitation(string email)
        {
            if(this.Invitations == null) this.Invitations = new List<Invitation>();
            this.Invitations.Add(new Invitation(email));
        }

        public void AddOrUpdateMembership(IApplicationUser user, Roles role = Roles.User)
        {
            if(this.Memberships == null) this.Memberships = new List<Membership>();
            var existing = this.Memberships.FirstOrDefault(m => string.Equals(m.UserId, user.Id ));

            this.Memberships.Add(new Membership(user.Id, user.UserName, role));
        }

        public bool IsInOrgansation(IApplicationUser user)
        {
            return this.Memberships?.Any(u => string.Equals(u.UserId, user.Id)) ?? false;
        }
    }
}