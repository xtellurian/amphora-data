using Amphora.Api.AspNet;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Profiles.Pages.Account
{
    [CommonAuthorize]
    public class LoginModel : PageModel
    {
        public LoginModel()
        { }
    }
}
