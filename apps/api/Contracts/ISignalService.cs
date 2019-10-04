using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Signals;
using Microsoft.Azure.TimeSeriesInsights.Models;
using Newtonsoft.Json.Linq;

namespace Amphora.Api.Contracts
{
    public interface ISignalService
    {
        Task<IEnumerable<QueryResultPage>> GetTsiSignalsAsync(AmphoraModel entity);
        Task<QueryResultPage> GetTsiSignalAsync(AmphoraModel entity, SignalModel signal, bool includeOtherSignals = false);
        Task WriteSignalAsync(AmphoraModel entity, JObject jObj);
        Task WriteSignalAsync(AmphoraModel entity, Dictionary<string, object> values);
    }
}