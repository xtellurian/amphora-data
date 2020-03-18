using System;
using Amphora.Common.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Identity.Models
{
    public class ApplicationUser : IdentityUser, IUser
    {
        public string? About { get; set; }
        public string? FullName { get; set; }
        // public bool IsAdminGlobal() => EmailConfirmed && (Email?.ToLower()?.EndsWith("@amphoradata.com") ?? false);
        // public virtual PinnedAmphorae PinnedAmphorae { get; set; } = new PinnedAmphorae(); // TODO: remove this feature for now

        // navigation
        // public string? OrganisationId { get; set; }
        // public virtual OrganisationModel? Organisation { get; set; }

        // public virtual ICollection<PurchaseModel> Purchases { get; set; } = new Collection<PurchaseModel>(); // never consumed
        public DateTimeOffset? LastModified { get; set; } // never consumed
        public DateTimeOffset? LastLoggedIn { get; set; } // never consumed

        public string? OrganisationId => null;
    }
}
