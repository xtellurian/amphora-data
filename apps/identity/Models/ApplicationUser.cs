using System;
using Amphora.Common.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Identity.Models
{
    public class ApplicationUser : IdentityUser, IUser
    {
        public string? About { get; set; }
        public string? FullName { get; set; }
        public DateTimeOffset? LastModified { get; set; } // never consumed
        public DateTimeOffset? LastLoggedIn { get; set; } // never consumed

        public string? OrganisationId => null;
    }
}
