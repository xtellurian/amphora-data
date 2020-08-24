using System.Threading.Tasks;
using Amphora.Api.Models.Dtos;
using Amphora.Common.Contracts;

namespace Amphora.Api.Controllers.Accounts
{
    public abstract class AccountControllerBase : EntityController
    {
        protected readonly IUserDataService userDataService;
        protected string OrganisationId { get; set; }

        protected AccountControllerBase(IUserDataService userDataService)
        {
            this.userDataService = userDataService;
        }

        // returns null if no need to return
        protected async Task<Microsoft.AspNetCore.Mvc.IActionResult> EnsureIdAsync(string id = null)
        {
            if (id == null)
            {
                var userData = await userDataService.ReadAsync(User);
                if (userData.Failed)
                {
                    return Handle(userData);
                }
                else if (string.IsNullOrEmpty(userData.Entity.OrganisationId))
                {
                    return StatusCode(406, new Response("User doesn't have an account or organisation"));
                }
                else
                {
                    id = userData.Entity.OrganisationId;
                }
            }

            OrganisationId = id;
            return null;
        }
    }
}