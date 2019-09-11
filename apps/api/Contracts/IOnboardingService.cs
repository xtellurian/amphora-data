using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Common.Models;
using Amphora.Common.Models.Organisations;

namespace Amphora.Api.Contracts
{
    public interface IOnboardingService
    {
        Task<EntityOperationResult<OrganisationModel>> CreateOrganisationAsync(ClaimsPrincipal principal, OrganisationModel org);
        Task<EntityOperationResult<IApplicationUser>> CreateUserAsync(IApplicationUser user, string password,  string onboardingId);
        Task<OrganisationModel> GetOrganisationFromOnboardingId(string onboardingId);
        Task<EntityOperationResult<OnboardingState>> InviteToOrganisation(ClaimsPrincipal principal, string email);
    }
}