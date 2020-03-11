using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Purchases;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Common.Models.Users
{
    public class ApplicationUser : IdentityUser, IUser
    {
        public string? About { get; set; }
        public string? FullName { get; set; }
        public bool IsAdminGlobal() => EmailConfirmed && (Email?.ToLower()?.EndsWith("@amphoradata.com") ?? false);
        public virtual PinnedAmphorae PinnedAmphorae { get; set; } = new PinnedAmphorae(); // TODO: remove this feature for now

        // navigation
        public string? OrganisationId { get; set; }
        public virtual OrganisationModel? Organisation { get; set; }

        public virtual ICollection<PurchaseModel> Purchases { get; set; } = new Collection<PurchaseModel>(); // never consumed
        public DateTimeOffset? LastModified { get; set; } // never consumed
        public DateTimeOffset? LastLoggedIn { get; set; } // never consumed
    }
}
