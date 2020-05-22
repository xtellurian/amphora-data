using System;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
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

        public bool? FirstTime { get; private set; }

        public async Task<IActionResult> OnGetAsync(bool? firstTime)
        {
            this.FirstTime = firstTime;
            await LoadPropertiesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string planType, bool? firstTime)
        {
            await LoadPropertiesAsync();
            var oldPlanType = Organisation?.Account?.Plan?.PlanType;
            var plan = Enum.Parse(typeof(Plan.PlanTypes), planType) as Plan.PlanTypes? ?? Plan.PlanTypes.Free;

            if (Organisation == null && plan != Plan.PlanTypes.Free)
            {
                // Plan is Team or Insitution
                return RedirectToPage("/Create", new { area = "Organisations", planType = plan.ToString(), firstTime = firstTime });
            }
            else if (Organisation == null && plan == Plan.PlanTypes.Free)
            {
                // IS the free plan, so just create a dummy org and go to quickstart
                var org = OrganisationModel.Autogenerate($"{UserData.ContactInformation?.FullName ?? "User"}'s Organisation");

                var createRes = await organisationService.CreateAsync(User, org);
                if (createRes.Succeeded)
                {
                    return RedirectToPage("/Quickstart");
                }
                else
                {
                    // uh ok
                    logger.LogCritical($"Failed to autogenerate org for user {UserData.UserName}. {createRes.Message}");
                    return RedirectToPage("/Create", new { area = "Organisations", planType = plan.ToString() });
                }
            }

            Organisation.Account ??= new Common.Models.Organisations.Accounts.Account();
            Organisation.Account.Plan ??= new Common.Models.Organisations.Accounts.Plan();

            switch (plan)
            {
                case Plan.PlanTypes.Team:
                    Organisation.Account.Plan.PlanType = Common.Models.Organisations.Accounts.Plan.PlanTypes.Team;
                    break;
                case Plan.PlanTypes.Institution:
                    Organisation.Account.Plan.PlanType = Common.Models.Organisations.Accounts.Plan.PlanTypes.Institution;
                    break;
                case Plan.PlanTypes.Free:
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

            if (firstTime == true)
            {
                return RedirectToPage("/Quickstart");
            }
            else
            {
                return RedirectToPage("./Plan");
            }
        }
    }
}