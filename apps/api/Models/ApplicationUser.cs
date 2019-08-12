using Amphora.Common.Contracts;
using ElCamino.AspNetCore.Identity.AzureTable.Model;

namespace Amphora.Api.Models
{
    public class ApplicationUser : IdentityUserV2, IOrgScoped
    {
        public string OrgId {get; set; }
    }
}