using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Models;

namespace Amphora.Api.Contracts
{
    public interface IAmphoraFileService
    {
        Task<EntityOperationResult<byte[]>> ReadFileAsync(ClaimsPrincipal principal, Common.Models.AmphoraModel entity, string file);
        Task<EntityOperationResult<byte[]>> WriteFileAsync(ClaimsPrincipal principal, Common.Models.AmphoraModel entity, byte[] contents, string file);
    }
}