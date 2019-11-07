using System;
using System.Collections.Generic;
using System.Linq;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Purchases;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Common.Models.Users
{
    public class ApplicationUser : IdentityUser, IUser
    {
        public string About { get; set; }
        public string FullName { get; set; }
        public bool? IsGlobalAdmin { get; set; }

        //navigation
        public string OrganisationId { get; set; }
        public virtual OrganisationModel Organisation { get; set; }

        public virtual ICollection<PurchaseModel> Purchases { get; set; }
        public Uri GetProfilePictureUri()
        {
            return new Uri($"https://www.gravatar.com/avatar/{GravatarExtensions.HashEmailForGravatar(this.Email)}");
        }

        public bool IsAdmin()
        {
            var membership = this.Organisation?.Memberships?.FirstOrDefault(m => m.UserId == this.Id);
            return membership?.Role == Roles.Administrator;
        }

        public bool GlobalAdmin()
        {
            return this.IsGlobalAdmin ?? false;
        }
    }
}
