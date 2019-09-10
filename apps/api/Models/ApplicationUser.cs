using Amphora.Common.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Models
{
    public interface IApplicationUser
    {
        string Id { get; set; }
        string OnboardingId { get; set; }
        bool IsOnboarding { get; set; }
        string UserName { get; set; }
        string Email { get; set; }
        string About { get; set; }
        string FullName { get; set; }
        string OrganisationId { get; set; }
        bool Validate();
    }

    public class ApplicationUser : Microsoft.AspNetCore.Identity.DocumentDB.IdentityUser, IApplicationUser
    {
        public string About { get; set; } = "I'm a happy cosmic doge";
        public string FullName { get; set; }
        public string OrganisationId { get; set; }
        public bool IsOnboarding { get; set; }
        public string OnboardingId { get; set; }

        public bool Validate()
        {
            if (IsOnboarding) return true;
            else return !string.IsNullOrEmpty(OrganisationId);
        }
    }

    public class TestApplicationUser : IdentityUser, IApplicationUser
    {
        public string About { get; set; }
        public string FullName { get; set; }
        public string OrganisationId { get; set; }
        public string OnboardingId { get; set; }
        public bool IsOnboarding { get; set; }

        public bool Validate()
        {
            if (IsOnboarding) return true;
            else return !string.IsNullOrEmpty(OrganisationId);
        }
    }
}