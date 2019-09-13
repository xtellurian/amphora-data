using Amphora.Common.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Models
{

    public class ApplicationUser : Microsoft.AspNetCore.Identity.DocumentDB.IdentityUser, IApplicationUser
    {
        public string About { get; set; } = "I'm a happy cosmic doge";
        public string FullName { get; set; }
        public string OrganisationId { get; set; }
        public string OnboardingId { get; set; }

    }

    public class TestApplicationUser : IdentityUser, IApplicationUser
    {
        public string About { get; set; }
        public string FullName { get; set; }
        public string OrganisationId { get; set; }

    }
}