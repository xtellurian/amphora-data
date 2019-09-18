using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages
{
    public class IndexModel : PageModel
    {

        public IndexModel()
        {
        }

        public IActionResult OnGet()
        {
           return Page();
        }
    }
}