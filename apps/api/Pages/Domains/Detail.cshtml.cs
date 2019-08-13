using System;
using System.Collections.Generic;
using Amphora.Common.Models.Domains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Domains
{
    public class DetailModel: PageModel
    {
        public Domain Domain {get;set;}
        public IActionResult OnGet(string id)
        {
            if(string.IsNullOrEmpty(id)) RedirectToPage("./Index");
            if( Enum.TryParse(id,  out DomainId domainId))
            {
                Domain = Domain.GetDomain(domainId);
                return Page();
            }
            else
            {
                return RedirectToPage("./Index");
            }
        }
    }
}