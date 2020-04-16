using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Permissions.Rules;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Areas.Amphorae.Pages.Detail
{
    [CommonAuthorize]
    public class AccessPageModel : AmphoraDetailPageModel
    {
        private readonly IAccessControlService accessControlService;
        private readonly IOrganisationService organisationService;
        private readonly IUserDataService userDataService;
        private readonly ILogger<AccessPageModel> logger;

        public AccessPageModel(IAmphoraeService amphoraeService,
                                IQualityEstimatorService qualityEstimator,
                                IPurchaseService purchaseService,
                                IPermissionService permissionService,
                                IAccessControlService accessControlService,
                                IOrganisationService organisationService,
                                IUserDataService userDataService,
                                ILogger<AccessPageModel> logger) : base(amphoraeService,
                                                                                 qualityEstimator,
                                                                                 purchaseService,
                                                                                 permissionService)
        {
            this.accessControlService = accessControlService;
            this.organisationService = organisationService;
            this.userDataService = userDataService;
            this.logger = logger;
        }

        public List<SelectListItem> AllowOrDenySelectList => new List<SelectListItem>
        {
            new SelectListItem("Deny", "Deny"),
            new SelectListItem("Allow", "Allow"),
        };

        [BindProperty]
        public Models.Dtos.AccessControls.UserAccessRule Rule { get; set; } = new Models.Dtos.AccessControls.UserAccessRule();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await LoadAmphoraAsync(id);
            await SetPagePropertiesAsync();
            return OnReturnPage();
        }

        public async Task<IActionResult> OnPostCreateRuleAsync(string id)
        {
            await LoadAmphoraAsync(id);
            await SetPagePropertiesAsync();
            EntityOperationResult<ApplicationUserDataModel> userReadRes;
            if (IsValidEmail(Rule.Username))
            {
                userReadRes = await userDataService.ReadFromEmailAsync(User, Rule.Username);
            }
            else
            {
                userReadRes = await userDataService.ReadFromUsernameAsync(User, Rule.Username);
            }

            if (userReadRes.Succeeded)
            {
                var rule = new UserAccessRule(Rule.GetKind(), Rule.Priority, userReadRes.Entity);
                var createRes = await accessControlService.CreateAsync(User, Amphora, rule);
                if (createRes.Succeeded)
                {
                    await LoadAmphoraAsync(id);
                    await SetPagePropertiesAsync();
                }
            }
            else if (await TryCreateForOrganisation())
            {
                // it worked for the org
                await LoadAmphoraAsync(id);
                await SetPagePropertiesAsync();
            }
            else
            {
                logger.LogWarning($"Failed to create rule for target {Rule.Username}");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteRuleAsync(string id, string ruleId)
        {
            await LoadAmphoraAsync(id);
            await SetPagePropertiesAsync();

            var deleteRes = await accessControlService.DeleteAsync(User, Amphora, ruleId);
            if (deleteRes.Succeeded)
            {
                await LoadAmphoraAsync(id);
                await SetPagePropertiesAsync();
            }
            else
            {
                ModelState.AddModelError(string.Empty, deleteRes.Message);
            }

            return Page();
        }

        private async Task<bool> TryCreateForOrganisation()
        {
            var orgReadRes = await organisationService.ReadAsync(User, Rule.Username); // use the field for the other one too
            if (orgReadRes.Succeeded)
            {
                var rule = new OrganisationAccessRule(Rule.GetKind(), Rule.Priority, orgReadRes.Entity);
                var createRes = await accessControlService.CreateAsync(User, Amphora, rule);
                if (createRes.Succeeded)
                {
                    return true;
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, createRes.Message);
                    return false;
                }
            }
            else
            {
                this.ModelState.AddModelError(string.Empty, orgReadRes.Message);
                return false;
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}