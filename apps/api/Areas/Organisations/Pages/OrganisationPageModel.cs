using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages
{
    public abstract class OrganisationPageModel : PageModel
    {
        protected readonly IOrganisationService organisationService;
        protected readonly IUserDataService userDataService;

        protected OrganisationPageModel(IOrganisationService organisationService, IUserDataService userDataService)
        {
            this.organisationService = organisationService;
            this.userDataService = userDataService;
        }

        public ApplicationUserDataModel UserData { get; private set; }
        public OrganisationModel Organisation { get; private set; }
        public bool IsAdmin => this.Organisation?.IsAdministrator(this.UserData) ?? false;

        protected async Task<bool> LoadPropertiesAsync()
        {
            var userDataReadRes = await userDataService.ReadAsync(User);
            if (userDataReadRes.Succeeded)
            {
                this.UserData = userDataReadRes.Entity;
                var readOrg = await organisationService.ReadAsync(User, UserData.OrganisationId);
                if (readOrg.Succeeded)
                {
                    this.Organisation = readOrg.Entity;
                }
            }

            return this.UserData != null && this.Organisation != null;
        }
    }
}