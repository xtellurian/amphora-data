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
        public virtual PinnedAmphorae PinnedAmphorae { get; set; } = new PinnedAmphorae();

        // navigation
        public string? OrganisationId { get; set; }
        public virtual OrganisationModel? Organisation { get; set; }

        public virtual ICollection<PurchaseModel> Purchases { get; set; } = new Collection<PurchaseModel>();
        public DateTimeOffset? LastModified { get; set; }
        public DateTimeOffset? LastLoggedIn { get; set; }

        public Uri GetProfilePictureUri()
        {
            return new Uri($"https://www.gravatar.com/avatar/{GravatarExtensions.HashEmailForGravatar(this.Email)}");
        }

        /// <summary>
        /// Returns whether the user is an admin of their organisation.
        /// </summary>
        public bool IsAdmin()
        {
            var membership = this.Organisation?.Memberships?.FirstOrDefault(m => m.UserId == this.Id);
            return membership?.Role == Roles.Administrator;
        }
    }
}
