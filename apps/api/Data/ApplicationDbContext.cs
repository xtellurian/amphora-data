using ElCamino.AspNetCore.Identity.AzureTable;
using ElCamino.AspNetCore.Identity.AzureTable.Model;

namespace Amphora.Api.Data
{

    public class ApplicationDbContext : IdentityCloudContext
    {
        public ApplicationDbContext() : base()
        {
        }

        public ApplicationDbContext(IdentityConfiguration config) : base(config)
        {
        }
    }
}