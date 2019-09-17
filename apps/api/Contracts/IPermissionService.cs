using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Permissions;

namespace Amphora.Api.Contracts
{
    public interface IPermissionService
    {
        Task<bool> IsAuthorizedAsync(IApplicationUser user, Common.Contracts.IEntity entity, AccessLevels accessLevel);
    }
}