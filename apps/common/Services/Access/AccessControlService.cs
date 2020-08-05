using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Emails;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Models.Permissions.Rules;

namespace Amphora.Common.Services.Access
{
    public class AccessControlService : IAccessControlService
    {
        private readonly IEntityStore<AmphoraAccessControlModel> store;
        private readonly IUserDataService userDataService;
        private readonly IPermissionService permissionService;
        private readonly IEmailSender emailSender;

        public AccessControlService(IEntityStore<AmphoraAccessControlModel> store,
                                    IUserDataService userDataService,
                                    IPermissionService permissionService,
                                    IEmailSender emailSender)
        {
            this.store = store;
            this.userDataService = userDataService;
            this.permissionService = permissionService;
            this.emailSender = emailSender;
        }

        public async Task<EntityOperationResult<AccessRule>> CreateAsync(ClaimsPrincipal principal, AmphoraModel amphora, AccessRule rule)
        {
            var userDataRes = await userDataService.ReadAsync(principal);
            if (userDataRes.Succeeded && userDataRes.Entity != null)
            {
                var userData = userDataRes.Entity;
                var authorized = await permissionService.IsAuthorizedAsync(userData, amphora, AccessLevels.Administer);
                if (authorized)
                {
                    if (amphora.AccessControl == null)
                    {
                        var ac = new AmphoraAccessControlModel(amphora);
                        ac = await store.CreateAsync(ac);
                        amphora.AccessControl = ac;
                    }

                    if (rule is UserAccessRule userRule)
                    {
                        amphora.AccessControl.UserAccessRules.Add(userRule);
                    }
                    else if (rule is OrganisationAccessRule orgRule)
                    {
                        amphora.AccessControl.OrganisationAccessRules.Add(orgRule);
                    }
                    else if (rule is AllRule allRule)
                    {
                        amphora.AccessControl.AllRule = allRule;
                    }
                    else
                    {
                        return new EntityOperationResult<AccessRule>("Unknown rule type");
                    }

                    var res = await store.UpdateAsync(amphora.AccessControl);
                    if (res == null)
                    {
                        return new EntityOperationResult<AccessRule>("An error occured when writing to the database.");
                    }

                    if (rule is UserAccessRule r && r?.Kind == Kind.Allow)
                    {
                        var content = await emailSender.Generator.ContentFromMarkdownTemplateAsync(
                            GivenAccessToAmphoraEmail.TemplateName,
                            GivenAccessToAmphoraEmail.GetTemplateData(amphora));
                        await emailSender.SendEmailAsync(new GivenAccessToAmphoraEmail(content, r.UserData?.ContactInformation?.Email, r.UserData?.ContactInformation?.FullName));
                    }

                    return new EntityOperationResult<AccessRule>(userData, rule);
                }
                else
                {
                    return new EntityOperationResult<AccessRule>("Unauthorized") { Code = 403, WasForbidden = true };
                }
            }
            else
            {
                return new EntityOperationResult<AccessRule>("Unknown User");
            }
        }

        public async Task<EntityOperationResult<AccessRule>> DeleteAsync(ClaimsPrincipal principal, AmphoraModel amphora, string ruleId)
        {
            var userDataRes = await userDataService.ReadAsync(principal);
            if (userDataRes.Succeeded)
            {
                var authorized = await permissionService.IsAuthorizedAsync(userDataRes.Entity!, amphora, AccessLevels.Administer);
                if (authorized)
                {
                    if (amphora.AccessControl == null)
                    {
                        // there's nothing to delete
                        return new EntityOperationResult<AccessRule>(true);
                    }

                    var rule = amphora.AccessControl?.Rules()?.FirstOrDefault(_ => _.Id == ruleId);
                    if (rule is null)
                    {
                        // rule doesn't exit
                        return new EntityOperationResult<AccessRule>($"Rule({ruleId}) doesn't exist");
                    }
                    else
                    {
                        if (rule is UserAccessRule userRule)
                        {
                            amphora.AccessControl!.UserAccessRules.Remove(userRule);
                        }
                        else if (rule is OrganisationAccessRule orgRule)
                        {
                            amphora.AccessControl!.OrganisationAccessRules.Remove(orgRule);
                        }
                        else if (rule is AllRule allRule)
                        {
                            amphora.AccessControl!.AllRule = null;
                        }
                        else
                        {
                            return new EntityOperationResult<AccessRule>("Unknown rule type");
                        }
                    }

                    var res = await store.UpdateAsync(amphora.AccessControl);
                    return new EntityOperationResult<AccessRule>(true);
                }
                else
                {
                    return new EntityOperationResult<AccessRule>("Unauthorized") { Code = 403, WasForbidden = true };
                }
            }
            else
            {
                return new EntityOperationResult<AccessRule>("Unknown User");
            }
        }
    }
}