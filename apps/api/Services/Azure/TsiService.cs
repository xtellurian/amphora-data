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
using System.Linq;

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

        public string GetDataAccessFqdn()
        {
            return this.options.CurrentValue.DataAccessFqdn;
        }

        public async Task InitAsync()
        {
            if(client != null) return;
            this.client = await GetTimeSeriesInsightsClientAsync();
        }

        public async Task<QueryResultPage> RunGetSeriesAsync(IList<object> ids,
                                                             IDictionary<string, Variable> variables,
                                                             DateTimeRange span,
                                                             IList<string> projections = null)
        {
            await InitAsync();
            string continuationToken;
            QueryResultPage queryResponse;
            do
            {
                // this method will return everything in ONE set (one graphed line). to split, call it twice
                queryResponse = await client.ExecuteQueryPagedAsync(
                   new QueryRequest(
                       getSeries: new Microsoft.Azure.TimeSeriesInsights.Models.GetSeries(
                           timeSeriesId: ids,
                           searchSpan: span,
                           filter: null,
                           projectedVariables: projections,
                           inlineVariables: variables)));


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