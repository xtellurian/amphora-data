using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Profiles.Pages.Account
{
    [Authorize]
    public class MissingModel : PageModel
    {

        public MissingModel()
        {
        }
    }
}