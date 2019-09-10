using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Common.Models;

namespace Amphora.Api.Contracts
{
    public interface IOnboardingService
    {
        Task<EntityOperationResult<Organisation>> CreateOrganisationAsync(ClaimsPrincipal principal, Organisation org);
        Task<EntityOperationResult<IApplicationUser>> CreateUserAsync(IApplicationUser user, string password,  string onboardingId);
        Task<Organisation> GetOrganisationFromOnboardingId(string onboardingId);
        Task<EntityOperationResult<OnboardingState>> InviteToOrganisation(ClaimsPrincipal principal, string email);
    }
}