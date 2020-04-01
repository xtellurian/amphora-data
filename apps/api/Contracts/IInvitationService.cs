using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Platform;

namespace Amphora.Api.Contracts
{
    public interface IInvitationService
    {
        IEntityStore<InvitationModel> Store { get; }

        Task<EntityOperationResult<InvitationModel>> AcceptInvitationAsync(ClaimsPrincipal principal, Common.Models.Platform.InvitationModel invitation);
        Task<EntityOperationResult<InvitationModel>> CreateInvitation(ClaimsPrincipal principal, InvitationModel invitation);
        Task<EntityOperationResult<InvitationModel>> GetInvitation(ClaimsPrincipal principal, string orgId);
        Task<InvitationModel> GetInvitationByEmailAsync(string email);
        Task<EntityOperationResult<IList<InvitationModel>>> GetMyInvitations(System.Security.Claims.ClaimsPrincipal principal);
    }
}