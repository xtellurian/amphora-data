using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Common.Contracts;

namespace Amphora.Api.Contracts
{
    public interface IUserService // this is for CRUD ops to apply permissions
    {
        IUserManager UserManager { get; }
        Task<EntityOperationResult<IApplicationUser>> CreateAsync(IApplicationUser user,
                                                                  string password);
        Task<EntityOperationResult<IApplicationUser>> DeleteAsync(IApplicationUser user);
    }
}