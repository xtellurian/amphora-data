using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Models.Feeds;
using Amphora.Common.Models;

namespace Amphora.Api.Contracts
{
    public interface IFeedAggregator
    {
        Task<EntityOperationResult<IEnumerable<IFeedEvent>>> GetFeedAsync(ClaimsPrincipal principal);
    }
}