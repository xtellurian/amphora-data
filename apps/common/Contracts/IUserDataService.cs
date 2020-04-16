using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Models;
using Amphora.Common.Models.Users;

namespace Amphora.Common.Contracts
{
    public interface IUserDataService
    {
        Task<EntityOperationResult<ApplicationUserDataModel>> ReadFromUsernameAsync(ClaimsPrincipal principal, string username);
        Task<EntityOperationResult<ApplicationUserDataModel>> ReadFromEmailAsync(ClaimsPrincipal principal, string email);
        IQueryable<ApplicationUserDataModel> Query(ClaimsPrincipal principal, Expression<Func<ApplicationUserDataModel, bool>> where);
        Task<EntityOperationResult<ApplicationUserDataModel>> ReadAsync(ClaimsPrincipal principal, string? userId = null);
        Task<EntityOperationResult<ApplicationUserDataModel>> CreateAsync(ClaimsPrincipal principal, ApplicationUserDataModel userData);
        Task<EntityOperationResult<ApplicationUserDataModel>> UpdateAsync(ClaimsPrincipal principal, ApplicationUserDataModel userData);
    }
}