using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.TimeSeriesInsights.Models;

namespace Amphora.Api.Contracts
{
    public interface ITsiService
    {
        Task<IList<TimeSeriesInstance>> GetInstancesAsync();
        Task<QueryResultPage> RunGetAggregateAsync(IList<object> ids, IDictionary<string, Variable> variables, DateTimeRange span, TimeSpan? interval = null, IList<string>? projections = null);
        Task<QueryResultPage> RunGetEventsAsync(IList<object> ids, DateTimeRange span, IList<string>? properties = null);
        Task<QueryResultPage> RunGetSeriesAsync(IList<object> ids,
                                                IDictionary<string, Variable> variables,
                                                DateTimeRange span,
                                                IList<string>? projections = null);
        Task<QueryResultPage> RunQueryAsync(QueryRequest query, string? continuationToken = null);
    }
}