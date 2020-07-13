using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Platform;
using Amphora.Common.Models.Purchases;
using Amphora.Common.Models.Users;

namespace Amphora.Common.Models.Organisations
{
    public class OrganisationModel : EntityBase, IEntity, ISearchable
    {
        public static OrganisationModel Autogenerate(string name)
        {
            return new OrganisationModel(name, "Automatically Created", null, null)
            {
                IsAutogenerated = true
            };
        }

        public OrganisationModel()
        {
            Name = null!;
        }

        public OrganisationModel(string name, string about, string? websiteUrl, string? address)
        {
            Name = name;
            About = about;
            WebsiteUrl = websiteUrl;
            Address = address;
        }

        public string Name { get; set; }
        public string? About { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? Address { get; set; }
        public bool? IsAutogenerated { get; set; }
        // owned
        public virtual Accounts.Account? Account { get; set; } = new Accounts.Account();
        public virtual Configuration? Configuration { get; set; } = new Configuration();
        public virtual DataCache? Cache { get; set; } = new DataCache();
        public virtual ICollection<InvitationModel> GlobalInvitations { get; set; } = new Collection<InvitationModel>();
        public virtual ICollection<Membership> Memberships { get; set; } = new Collection<Membership>();
        // navigation
        public virtual ICollection<TermsOfUseModel> TermsOfUses { get; set; } = new Collection<TermsOfUseModel>();
        public virtual ICollection<TermsOfUseAcceptanceModel> TermsOfUsesAccepted { get; set; } = new Collection<TermsOfUseAcceptanceModel>();
        public virtual ICollection<PurchaseModel> Purchases { get; set; } = new Collection<PurchaseModel>();
        public string? CreatedById { get; set; }
        public virtual ApplicationUserDataModel? CreatedBy { get; set; }

        public void AddOrUpdateMembership(ApplicationUserDataModel user, Roles role = Roles.User)
        {
            if (this.Memberships == null) { this.Memberships = new List<Membership>(); }
            var allMembers = this.Memberships; // so they are tracked.
            var existing = allMembers.FirstOrDefault(m => string.Equals(m.UserId, user.Id));
            if (existing == null)
            {
                this.Memberships.Add(new Membership(user, role));
            }
            else
            {
                existing.Role = role;
            }
        }

        public bool IsInOrgansation(IUser user)
        {
            return this.Memberships?.Any(u => string.Equals(u.UserId, user.Id)) ?? false;
        }

        public bool IsAdministrator(IUser user)
        {
            if (user == null)
            {
                return false;
            }

            return this.IsAdministrator(user.Id);
        }

        public bool IsAdministrator(string userId)
        {
            var membership = this.Memberships?.FirstOrDefault(m => m.UserId == userId);
            return membership?.Role == Roles.Administrator;
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