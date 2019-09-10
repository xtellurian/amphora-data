using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Organisations
{
    [Authorize]
    public class DetailModel : PageModel
    {
        private readonly IEntityStore<Organisation> entityStore;
        private readonly IUserManager userManager;

        public DetailModel(IEntityStore<Organisation> entityStore, IUserManager userManager)
        {
            this.entityStore = entityStore;
            this.userManager = userManager;
        }
        public Organisation Organisation { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if(string.IsNullOrEmpty(id)) 
            {
                var user = await userManager.GetUserAsync(User);
                this.Organisation = await entityStore.ReadAsync(user.OrganisationId);
            }
            else
            {
                this.Organisation = await entityStore.ReadAsync(id);
            }
            if(this.Organisation == null ) return RedirectToPage("/Index");
            return Page();
        }
    }
}