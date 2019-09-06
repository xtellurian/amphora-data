using System.Security.Claims;
using System.Threading.Tasks;

namespace Amphora.Api.Contracts
{
    public interface IAmphoraeService
    {
        Task<Models.EntityOperationResult<Common.Models.Amphora>> CreateAsync(Common.Models.Amphora model, ClaimsPrincipal creator);
    }
}