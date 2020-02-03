using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Models.Platform;
using Amphora.Common.Models.Purchases;
using Amphora.Common.Models.Users;

namespace Amphora.Common.Models.Organisations
{
    public class OrganisationModel : EntityBase, IEntity
    {
        public OrganisationModel()
        {
            Name = null!;
            About = null!;
            WebsiteUrl = null!;
            Address = null!;
        }

        public OrganisationModel(string name, string about, string websiteUrl, string address)
        {
            Name = name;
            About = about;
            WebsiteUrl = websiteUrl;
            Address = address;
        }

        public string Name { get; set; }
        public string About { get; set; }
        public string WebsiteUrl { get; set; }
        public string Address { get; set; }
        // owned
        public virtual Accounts.Account Account { get; set; } = new Accounts.Account();
        public virtual ICollection<InvitationModel> GlobalInvitations { get; set; } = new Collection<InvitationModel>();
        public virtual ICollection<Membership> Memberships { get; set; } = new Collection<Membership>();
        public virtual ICollection<RestrictionModel> Restrictions { get; set; } = new Collection<RestrictionModel>();
        public virtual PinnedAmphorae? PinnedAmphorae { get; set; } = new PinnedAmphorae();
        // navigation
        public virtual ICollection<TermsAndConditionsModel> TermsAndConditions { get; set; } = new Collection<TermsAndConditionsModel>();
        public virtual ICollection<TermsAndConditionsAcceptanceModel> TermsAndConditionsAccepted { get; set; } = new Collection<TermsAndConditionsAcceptanceModel>();
        public virtual ICollection<PurchaseModel> Purchases { get; set; } = new Collection<PurchaseModel>();
        public string? CreatedById { get; set; }
        public virtual ApplicationUser? CreatedBy { get; set; }

        public void AddOrUpdateMembership(ApplicationUser user, Roles role = Roles.User)
        {
            if (this.Memberships == null) { this.Memberships = new List<Membership>(); }
            var existing = this.Memberships.FirstOrDefault(m => string.Equals(m.UserId, user.Id));

            this.Memberships.Add(new Membership(user, role));
        }

        public bool IsInOrgansation(IUser user)
        {
            return this.Memberships?.Any(u => string.Equals(u.UserId, user.Id)) ?? false;
        }

        public bool AddTermsAndConditions(TermsAndConditionsModel model)
        {
            if (this.TermsAndConditions == null) { this.TermsAndConditions = new List<TermsAndConditionsModel>(); }
            if (this.TermsAndConditions.Any(t => t.Id == model.Id))
            {
                return false;
            }

            this.TermsAndConditions.Add(model);
            return true;
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