using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Admin.Pages.Accounts.Detail
{
    public class PlanPageModel : AccountDetailPageModel
    {
        public PlanPageModel(IEntityStore<OrganisationModel> orgStore) : base(orgStore)
        {
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (await LoadOrganisationAsync(id))
            {
                return Page();
            }
            else
            {
                return BadRequest(Error);
            }
        }

        public async Task<IActionResult> OnPostFreeAsync(string id)
        {
            if (await LoadOrganisationAsync(id))
            {
                this.Org.Account ??= new Account();
                this.Org.Account.Plan ??= new Plan();
                this.Org.Account.Plan.PlanType = Plan.PlanTypes.Free;
                Org = await orgStore.UpdateAsync(Org);
                return Page();
            }
            else
            {
                return BadRequest(Error);
            }
        }

        public async Task<IActionResult> OnPostTeamAsync(string id)
        {
            if (await LoadOrganisationAsync(id))
            {
                this.Org.Account ??= new Account();
                this.Org.Account.Plan ??= new Plan();
                this.Org.Account.Plan.PlanType = Plan.PlanTypes.Team;
                Org = await orgStore.UpdateAsync(Org);
                return Page();
            }
            else
            {
                return BadRequest(Error);
            }
        }

        public async Task<IActionResult> OnPostInstitutionAsync(string id)
        {
            if (await LoadOrganisationAsync(id))
            {
                this.Org.Account ??= new Account();
                this.Org.Account.Plan ??= new Plan();
                this.Org.Account.Plan.PlanType = Plan.PlanTypes.Institution;
                Org = await orgStore.UpdateAsync(Org);
                return Page();
            }
            else
            {
                return BadRequest(Error);
            }
        }
    }
}