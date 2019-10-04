using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.TimeSeriesInsights.Models;

namespace Amphora.Api.Contracts
{
    public interface ITsiService
    {
        Task<QueryResultPage> RunGetSeriesAsync(IList<object> ids,
                                                IDictionary<string, Variable> variables,
                                                DateTimeRange span,
                                                IList<string> projections = null);
    }
}