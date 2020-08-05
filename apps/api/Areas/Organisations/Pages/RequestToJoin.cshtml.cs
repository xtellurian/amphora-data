using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Emails;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages
{
    [CommonAuthorize]
    public class RequestToJoinPageModel : PageModel
    {
        private readonly IOrganisationService orgService;
        private readonly IUserDataService userDataService;
        private readonly IEmailSender emailSender;

        public RequestToJoinPageModel(IOrganisationService orgService, IUserDataService userDataService, IEmailSender emailSender)
        {
            this.orgService = orgService;
            this.userDataService = userDataService;
            this.emailSender = emailSender;
        }

        public OrganisationModel Organisation { get; private set; }
        public bool Succeeded { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var res = await orgService.ReadAsync(User, id);
            if (res.Succeeded)
            {
                this.Organisation = res.Entity;
            }
            else
            {
                return RedirectToPage("./Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var res = await orgService.ReadAsync(User, id);
            var userDataReadRes = await userDataService.ReadAsync(User);
            if (userDataReadRes.Succeeded)
            {
                var userData = userDataReadRes.Entity;
                if (res.Succeeded)
                {
                    this.Organisation = res.Entity;
                }
                else
                {
                    return RedirectToPage("./Index");
                }

                this.Succeeded = await emailSender.SendEmailAsyncV1(new RequestToJoinEmail(userData, res.Entity));
                return Page();
            }
            else
            {
                this.ModelState.AddModelError(string.Empty, userDataReadRes.Message);
                return Page();
            }
        }
    }
}