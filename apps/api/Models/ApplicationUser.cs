using Amphora.Common.Contracts;
using ElCamino.AspNetCore.Identity.AzureTable.Model;

namespace Amphora.Api.Models
{
    public class ApplicationUser : IdentityUserV2, IOrgScoped
    {
        public string About { get; set; } = "I'm a happy little doge";
        public string OrgId { get; set; }
    }
}