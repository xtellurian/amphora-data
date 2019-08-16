using System.Collections.Generic;
using Amphora.Common.Models.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Domains
{
    [Authorize]
    public class IndexModel: PageModel
    {
        public List<Domain> Domains {get;set;}
        public IActionResult OnGet()
        {
            Domains = Domain.GetAllDomains();
            return Page();
        }
    }
}