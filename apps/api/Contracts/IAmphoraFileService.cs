using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Amphorae.Files;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Contracts
{
    public interface IAmphoraFileService
    {
        IBlobStore<AmphoraModel> Store { get; }
        Task<EntityOperationResult<UploadResponse>> CreateFileAsync(ClaimsPrincipal principal, AmphoraModel entity, string file);
        Task<EntityOperationResult<object>> DeleteFileAsync(ClaimsPrincipal principal, AmphoraModel entity, string file);
        Task<EntityOperationResult<byte[]>> ReadFileAsync(ClaimsPrincipal principal, AmphoraModel entity, string file);
        Task<EntityOperationResult<UploadResponse>> WriteFileAsync(ClaimsPrincipal principal, AmphoraModel entity, byte[] contents, string file);
    }
}