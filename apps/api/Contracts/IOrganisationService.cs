using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Common.Models.Organisations;

namespace Amphora.Api.Contracts
{
    public interface IOrganisationService
    {
        IEntityStore<OrganisationModel> Store { get; }
        Task<EntityOperationResult<TermsAndConditionsAcceptanceModel>> AgreeToTermsAndConditions(ClaimsPrincipal principal, TermsAndConditionsModel termsAndConditions);
        Task<EntityOperationResult<OrganisationModel>> CreateOrganisationAsync(ClaimsPrincipal principal, OrganisationModel org);
        Task<EntityOperationResult<OrganisationModel>> ReadAsync(ClaimsPrincipal principal, string id);
        Task<byte[]> ReadrofilePictureJpg(OrganisationModel organisation);
        Task<EntityOperationResult<OrganisationModel>> UpdateAsync(ClaimsPrincipal principal, OrganisationModel org);
        Task WriteProfilePictureJpg(OrganisationModel organisation, byte[] bytes);
    }
}