using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Models.Organisations;

namespace Amphora.Api.Contracts
{
    public interface IOrganisationService
    {
        IEntityStore<OrganisationModel> Store { get; }
        Task<bool> AcceptInvitation(ClaimsPrincipal principal, string orgId);
        Task<Models.EntityOperationResult<OrganisationModel>> CreateOrganisationAsync(ClaimsPrincipal principal, OrganisationModel org);
        Task InviteToOrganisationAsync(ClaimsPrincipal principal, string orgId, string email);
    }
}