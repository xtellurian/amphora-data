using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Organisations
{
    public class AcceptInvitationModel: PageModel
    {
        private readonly IOrganisationService organisationService;

        public AcceptInvitationModel(IOrganisationService organisationService)
        {
            this.organisationService = organisationService;
        }
        public OrganisationModel Organisation {get; set;}
        public bool InvitationAccepted { get; private set; }

        public async Task<IActionResult> OnGetAsync(string organisationId)
        {
            this.Organisation = await organisationService.Store.ReadAsync(organisationId, organisationId);
            if(this.Organisation == null) return RedirectToPage("./Index");
            InvitationAccepted = await organisationService.AcceptInvitation(User, Organisation.OrganisationId);
            return Page();
        }
    }
}