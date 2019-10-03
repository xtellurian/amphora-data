using System;
using System.Collections.Generic;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Transactions;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Common.Models.Users
{
    public class ApplicationUser : IdentityUser, IUser
    {
        public string OrganisationId { get; set; }
        public OrganisationModel Organisation { get; set; }
        public string About { get; set; }
        public string FullName { get; set; }
        public List<TransactionModel> Transactions { get; set; }
        public Uri GetProfilePictureUri()
        {
            return new Uri($"https://www.gravatar.com/avatar/{GravatarExtensions.HashEmailForGravatar(this.Email)}");
        }
    }
}