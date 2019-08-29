using Amphora.Common.Contracts;

namespace Amphora.Api.Models
{
    public class ApplicationUser : Microsoft.AspNetCore.Identity.DocumentDB.IdentityUser, IOrgScoped
    {
        public string About { get; set; } = "I'm a happy cosmic doge";
        public string OrgId { get; set; }
        public string FullName { get; set; }
    }
}