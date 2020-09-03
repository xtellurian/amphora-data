using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Microsoft.Azure.TimeSeriesInsights.Models;

namespace Amphora.Api.Services.InMemory
{
    public class InMemoryTsiService : ITsiService
    {
        private static QueryResultPage Results(IList<DateTime?> t, IList<PropertyValues> p) => new QueryResultPage(null, t, p);
        private static IList<DateTime?> Timestamps()
        {
            var start = DateTime.UtcNow.AddDays(-7);
            var timestamps = new List<DateTime?>();
            for (var t = 0; t <= 8 * 24; t++)
            {
                timestamps.Add(start);
                start = start.AddHours(1);
            }

            return timestamps;
        }

        private static IList<PropertyValues> Properties(IEnumerable<string> names)
        {
            var properties = new List<PropertyValues>();
            var rand = new Random();
            foreach (var n in names)
            {
                var values = new List<object>();
                for (var t = 0; t <= 8 * 24; t++)
                {
                    values.Add(rand.NextDouble() * 100);
                }

                properties.Add(new PropertyValues(n, "Numeric", values));
            }

            return properties;
        }

        public Task<IList<TimeSeriesInstance>> GetInstancesAsync()
        {
            return Task.FromResult<IList<TimeSeriesInstance>>(new List<TimeSeriesInstance>());
        }

        public Task<QueryResultPage> RunGetAggregateAsync(IList<object> ids, IDictionary<string, Variable> variables, DateTimeRange span, TimeSpan? interval = null, IList<string> projections = null)
        {
            return Task.FromResult(Results(Timestamps(), Properties(variables.Keys)));
        }

        public Task<QueryResultPage> RunGetEventsAsync(IList<object> ids, DateTimeRange span, IList<string> properties = null)
        {
            return Task.FromResult(Results(Timestamps(), Properties(properties)));
        }

        public Task<QueryResultPage> RunGetSeriesAsync(IList<object> ids, IDictionary<string, Variable> variables, DateTimeRange span, IList<string> projections = null)
        {
            return Task.FromResult(Results(Timestamps(), Properties(variables.Keys)));
        }

        public Task<QueryResultPage> RunQueryAsync(QueryRequest query, string continuationToken = null)
        {
            if (query.AggregateSeries?.InlineVariables != null)
            {
                return Task.FromResult(Results(Timestamps(), Properties(query.AggregateSeries.InlineVariables.Keys)));
            }
            else if (query.GetSeries?.InlineVariables != null)
            {
                return Task.FromResult(Results(Timestamps(), Properties(query.GetSeries.InlineVariables.Keys)));
            }
            else if (query.GetEvents?.ProjectedProperties != null)
            {
                return Task.FromResult(Results(Timestamps(), Properties(query.GetEvents.ProjectedProperties.Select(_ => _.Name))));
            }
            else
            {
                throw new Exception("Everything was empty");
            }
        }
    }
}