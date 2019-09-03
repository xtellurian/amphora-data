using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Organisations
{
    public class DetailModel : PageModel
    {
        private readonly IEntityStore<Organisation> entityStore;

        public DetailModel(IEntityStore<Organisation> entityStore)
        {
            this.entityStore = entityStore;
        }
        public Organisation Organisation { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if(string.IsNullOrEmpty(id)) return RedirectToPage("/Index");
            this.Organisation = await entityStore.ReadAsync(id);
            if(this.Organisation == null ) return RedirectToPage("/Index");
            return Page();
        }
    }
}