using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Microsoft.Azure.TimeSeriesInsights.Models;

namespace Amphora.Api.Contracts
{
    public interface ISignalService
    {
        Task<QueryResultPage> GetTsiSignalAsync(ClaimsPrincipal principal, AmphoraModel entity, SignalV2 signal, bool includeOtherSignals = false);
        Task<EntityOperationResult<Dictionary<string, object>>> WriteSignalAsync(ClaimsPrincipal principal, AmphoraModel entity, Dictionary<string, object> values);
        Task<EntityOperationResult<IEnumerable<Dictionary<string, object>>>> WriteSignalBatchAsync(ClaimsPrincipal principal, AmphoraModel entity, IEnumerable<Dictionary<string, object>> values);
        Task<IDictionary<SignalV2, IEnumerable<string>>> GetUniqueValuesForStringProperties(ClaimsPrincipal principal, AmphoraModel entity);
        Task<long> MaxWriteTimeAsync(AmphoraModel entity);
    }
}
