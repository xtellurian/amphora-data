using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Platform;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Organisations.Pages.Account
{
    [CommonAuthorize]
    public class IndexModel : OrganisationPageModel
    {
        private readonly IInvitationService invitationService;

        public IndexModel(IUserDataService userDataService, IOrganisationService organisationService, IInvitationService invitationService)
        : base(organisationService, userDataService)
        {
            this.invitationService = invitationService;
        }

        public Amphora.Common.Models.Organisations.Accounts.Account Account { get; private set; }
        public InvitationModel Invitation { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (await LoadPropertiesAsync())
            {
                this.Account = Organisation.Account;
                this.Invitation = (await invitationService.GetMyInvitations(User)).Entity?
                    .FirstOrDefault(_ => _.State == InvitationState.Open);
                return Page();
            }
            else
            {
                return NotFound(Error);
            }
        }
    }
}