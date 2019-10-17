using System.Collections.Generic;
using System.Linq;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Users;

namespace Amphora.Common.Models.Organisations
{
    public class OrganisationModel : Entity, IEntity
    {
        public string Name { get; set; }
        public string About { get; set; }
        public string WebsiteUrl { get; set; }
        public string Address { get; set; }

        // navigation
        public virtual ICollection<Invitation> Invitations { get; set; }
        public virtual ICollection<Membership> Memberships { get; set; }
        public virtual ICollection<TermsAndConditionsModel> TermsAndConditions { get; set; }
        public virtual ICollection<TermsAndConditionsAcceptanceModel> TermsAndConditionsAccepted { get; set; }
        public string CreatedById { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }

        public void AddInvitation(string email)
        {
            if (this.Invitations == null) this.Invitations = new List<Invitation>();
            this.Invitations.Add(new Invitation(email));
        }

        public void AddOrUpdateMembership(ApplicationUser user, Roles role = Roles.User)
        {
            if (this.Memberships == null) this.Memberships = new List<Membership>();
            var existing = this.Memberships.FirstOrDefault(m => string.Equals(m.UserId, user.Id));

            this.Memberships.Add(new Membership(user, role));
        }

        public bool IsInOrgansation(IUser user)
        {
            return this.Memberships?.Any(u => string.Equals(u.UserId, user.Id)) ?? false;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Name)
                && this.CreatedDate.HasValue;
        }

        // Address
        // Registration Number -- like ACN or ABN or something
        // billing thing here
        // current discount program / incentives...
    }
}