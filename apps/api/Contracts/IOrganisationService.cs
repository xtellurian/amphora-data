using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Models.Organisations;

namespace Amphora.Api.Contracts
{
    public interface IOrganisationService
    {
        IEntityStore<OrganisationModel> Store { get; }
        Task<bool> AcceptInvitation(ClaimsPrincipal principal, string orgId);
        Task<Models.EntityOperationResult<OrganisationExtendedModel>> CreateOrganisationAsync(ClaimsPrincipal principal, OrganisationExtendedModel org);
        Task InviteToOrganisationAsync(ClaimsPrincipal principal, string orgId, string email);
        Task<string> ProfilePictureUrl(OrganisationModel organisation);
        Task WriteProfilePictureJpg(OrganisationModel organisation, byte[] bytes);
    }
}