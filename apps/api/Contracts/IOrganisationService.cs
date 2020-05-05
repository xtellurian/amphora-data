using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;

namespace Amphora.Api.Contracts
{
    public interface IOrganisationService
    {
        IEntityStore<OrganisationModel> Store { get; }
        Task<EntityOperationResult<OrganisationModel>> CreateAsync(ClaimsPrincipal principal, OrganisationModel org);
        Task<EntityOperationResult<OrganisationModel>> DeleteAsync(ClaimsPrincipal principal, OrganisationModel model);
        Task<EntityOperationResult<OrganisationModel>> ReadAsync(ClaimsPrincipal principal, string id);
        Task<byte[]> ReadrofilePictureJpg(OrganisationModel organisation);
        Task<EntityOperationResult<OrganisationModel>> UpdateAsync(ClaimsPrincipal principal, OrganisationModel org);
        Task WriteProfilePictureJpg(OrganisationModel organisation, byte[] bytes);
    }
}