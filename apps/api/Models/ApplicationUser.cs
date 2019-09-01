using Amphora.Common.Contracts;

namespace Amphora.Api.Models
{
    public class ApplicationUser : Microsoft.AspNetCore.Identity.DocumentDB.IdentityUser, IEntity
    {
        public string About { get; set; } = "I'm a happy cosmic doge";
        public string FullName { get; set; }
        public string OrganisationId { get; set; }
    }
}