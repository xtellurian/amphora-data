using System.Collections.Generic;
using System.Collections.ObjectModel;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Purchases;

namespace Amphora.Common.Models.Users
{
    public class ApplicationUserDataModel : EntityBase, IEntity, IUser
    {
        public ApplicationUserDataModel()
        { }

        public ApplicationUserDataModel(string id, string userName, string about, ContactInformation contactInformation)
        {
            Id = id;
            UserName = userName;
            About = about;
            ContactInformation = contactInformation;
        }

        public string? UserName { get; set; }
        public string? About { get; set; }
        public string? PhoneNumber { get; set; }
        public System.DateTimeOffset? LastSeen { get; set; }

        public virtual ContactInformation? ContactInformation { get; set; } = new ContactInformation();
        // navigation
        public string? OrganisationId { get; set; }
        public virtual OrganisationModel? Organisation { get; set; }

        public virtual ICollection<PurchaseModel> Purchases { get; set; } = new Collection<PurchaseModel>();
    }
}