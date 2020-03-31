using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Areas.Organisations.Pages.Account
{
    [CommonAuthorize]
    public class SelectPlanPageModel : OrganisationPageModel
    {
        private readonly ILogger<SelectPlanPageModel> logger;

        public SelectPlanPageModel(IOrganisationService organisationService,
                                   IUserDataService userDataService,
                                   ILogger<SelectPlanPageModel> logger) : base(organisationService, userDataService)
        {
            this.logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (await LoadPropertiesAsync())
            {
                return Page();
            }
            else
            {
                // something went wrong
                logger.LogError("Failed to load properties");
                return RedirectToPage("../Detail");
            }
        }

        public async Task<IActionResult> OnPostAsync(string plan)
        {
            await LoadPropertiesAsync();

            var oldPlanType = Organisation?.Account?.Plan?.PlanType;
            Organisation.Account ??= new Common.Models.Organisations.Accounts.Account();
            Organisation.Account.Plan ??= new Common.Models.Organisations.Accounts.Plan();
            switch (plan?.ToLower())
            {
                case "team":
                    Organisation.Account.Plan.PlanType = Common.Models.Organisations.Accounts.Plan.PlanTypes.Team;
                    break;
                case "institution":
                    Organisation.Account.Plan.PlanType = Common.Models.Organisations.Accounts.Plan.PlanTypes.Institution;
                    break;
                case "free":
                    Organisation.Account.Plan.PlanType = Common.Models.Organisations.Accounts.Plan.PlanTypes.Free;
                    break;
                default:
                    Organisation.Account.Plan.PlanType = Common.Models.Organisations.Accounts.Plan.PlanTypes.Free;
                    break;
            }

            if (oldPlanType != Organisation.Account.Plan.PlanType)
            {
                // updating plan
                var updateRes = await organisationService.UpdateAsync(User, this.Organisation);
                if (!updateRes.Succeeded)
                {
                    this.ModelState.AddModelError(string.Empty, updateRes.Message);
                    return Page();
                }
            }

            return RedirectToPage("./Plan");
        }
    }
}