using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Signals;
using Microsoft.Azure.TimeSeriesInsights.Models;

namespace Amphora.Api.Contracts
{
    public interface ISignalService
    {
        Task<IEnumerable<QueryResultPage>> GetTsiSignalsAsync(ClaimsPrincipal principal, AmphoraModel entity);
        Task<QueryResultPage> GetTsiSignalAsync(ClaimsPrincipal principal, AmphoraModel entity, SignalModel signal, bool includeOtherSignals = false);
        Task<EntityOperationResult<Dictionary<string, object>>> WriteSignalAsync(ClaimsPrincipal principal, AmphoraModel entity, Dictionary<string, object> values);
        Task<EntityOperationResult<IEnumerable<Dictionary<string, object>>>> WriteSignalBatchAsync(ClaimsPrincipal principal, AmphoraModel entity, IEnumerable<Dictionary<string, object>> values);
        Task<IDictionary<SignalModel, IEnumerable<string>>> GetUniqueValuesForStringProperties(ClaimsPrincipal principal, AmphoraModel entity);
        Task<EntityOperationResult<SignalModel>> AddSignal(ClaimsPrincipal principal, AmphoraModel amphora, SignalModel signal);
    }
}
