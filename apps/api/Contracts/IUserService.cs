using System.Threading.Tasks;
using Amphora.Api.Models;

namespace Amphora.Api.Contracts
{
    public interface IUserService
    {
        Task<EntityOperationResult<ApplicationUser>> CreateUserAsync(ApplicationUser user, string password);
    }
}