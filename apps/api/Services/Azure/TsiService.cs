using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.TimeSeriesInsights;
using Microsoft.Azure.TimeSeriesInsights.Models;
using Microsoft.Rest;
using DateTimeRange = Microsoft.Azure.TimeSeriesInsights.Models.DateTimeRange;
using Amphora.Api.Contracts;
using Microsoft.Extensions.Options;
using Amphora.Api.Options;

namespace Amphora.Api.Services.Azure
{
    public class TsiService : ITsiService
    {
        private const string resource = "https://api.timeseries.azure.com/";
        private readonly IOptionsMonitor<TsiOptions> options;
        private readonly IAzureServiceTokenProvider tokenProvider;
        private ITimeSeriesInsightsClient client;

        public TsiService(IOptionsMonitor<TsiOptions> options, IAzureServiceTokenProvider tokenProvider)
        {
            this.options = options;
            this.tokenProvider = tokenProvider;
        }

        public async Task<QueryResultPage> FullSet(string id, string property, DateTime start, DateTime end)
        {
            await InitAsync();
            var range = new DateTimeRange(start, end);
            return await RunGetSeriesAsync(new List<object>{id}, range, property);
        }

        public string GetDataAccessFqdn()
        {
            return this.options.CurrentValue.DataAccessFqdn;
        }

        public async Task InitAsync()
        {
            this.client = await GetTimeSeriesInsightsClientAsync();
        }

        private async Task<QueryResultPage> RunGetSeriesAsync(IList<object> ids, DateTimeRange span, string property)
        {
            string continuationToken;
            QueryResultPage queryResponse;
            do
            {
                queryResponse = await client.ExecuteQueryPagedAsync(
                   new QueryRequest(
                       getSeries: new Microsoft.Azure.TimeSeriesInsights.Models.GetSeries(
                           timeSeriesId: ids,
                           searchSpan: span,
                           filter: null,
                           projectedVariables: new[] { "Avg" },
                           inlineVariables: new Dictionary<string, Variable>()
                           {
                               ["Avg"] = new NumericVariable(
                                   value: new Tsx($"$event.{property}"),
                                   aggregation: new Tsx("avg($value)"))
                           })));

                continuationToken = queryResponse.ContinuationToken;
            }
            while (continuationToken != null);

            return queryResponse;
        }

        private async Task<ITimeSeriesInsightsClient> GetTimeSeriesInsightsClientAsync()
        {
            var token = await tokenProvider.GetAccessTokenAsync(resource);
            var serviceClientCredentials = new TokenCredentials(token);

            var timeSeriesInsightsClient = new Microsoft.Azure.TimeSeriesInsights.TimeSeriesInsightsClient(credentials: serviceClientCredentials)
            {
                EnvironmentFqdn = options.CurrentValue.DataAccessFqdn
            };
            return timeSeriesInsightsClient;
        }
    }
}