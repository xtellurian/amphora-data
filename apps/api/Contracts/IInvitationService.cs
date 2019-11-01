using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Models.Platform;

namespace Amphora.Api.Contracts
{
    public interface IInvitationService
    {
        IEntityStore<InvitationModel> Store { get; }

        Task<Models.EntityOperationResult<InvitationModel>> AcceptInvitationAsync(ClaimsPrincipal principal, Common.Models.Platform.InvitationModel invitation);
        Task<Models.EntityOperationResult<InvitationModel>> CreateInvitation(ClaimsPrincipal principal, InvitationModel invitation, bool inviteToOrg = false);
        Task<Models.EntityOperationResult<InvitationModel>> GetInvitation(ClaimsPrincipal principal, string orgId);
        Task<InvitationModel> GetInvitationByEmailAsync(string email);
        Task<Models.EntityOperationResult<IList<InvitationModel>>> GetMyInvitations(System.Security.Claims.ClaimsPrincipal principal);
    }
}