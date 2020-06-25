using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Amphorae.Files;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Contracts
{
    public interface IAmphoraFileService
    {
        IAmphoraBlobStore Store { get; }
        Task<EntityOperationResult<UploadResponse>> CreateFileAsync(ClaimsPrincipal principal, AmphoraModel entity, string file);
        Task<EntityOperationResult<object>> DeleteFileAsync(ClaimsPrincipal principal, AmphoraModel entity, string file);
        Task<EntityOperationResult<IDictionary<string, string>>> ReadAttributesAsync(ClaimsPrincipal principal, AmphoraModel entity, string path);
        Task<EntityOperationResult<byte[]>> ReadFileAsync(ClaimsPrincipal principal, AmphoraModel entity, string file);
        Task<EntityOperationResult<WriteAttributesResponse>> WriteAttributesAsync(ClaimsPrincipal principal, AmphoraModel entity, IDictionary<string, string> attributes, string path);
        Task<EntityOperationResult<UploadResponse>> WriteFileAsync(ClaimsPrincipal principal, AmphoraModel entity, byte[] contents, string file);
    }
}