using Amphora.Common.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Models
{
    public interface IApplicationUser
    {
        string Id { get; set; }
        string UserName { get; set; }
        string Email { get; set; }
        string About { get; set; }
        string FullName { get; set; }
        string OrganisationId { get; set; }
    }

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