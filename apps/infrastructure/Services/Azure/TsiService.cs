using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Options;
using Amphora.Infrastructure.Models;
using Microsoft.Azure.TimeSeriesInsights;
using Microsoft.Azure.TimeSeriesInsights.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Rest;
using DateTimeRange = Microsoft.Azure.TimeSeriesInsights.Models.DateTimeRange;

namespace Amphora.Infrastructure.Services.Azure
{
    public class TsiService : ITsiService
    {
        private const string Resource = "https://api.timeseries.azure.com/";
        private readonly IOptionsMonitor<TsiOptions> options;
        private readonly IAzureServiceTokenProvider tokenProvider;
        private readonly ITelemetry telemetry;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly ILogger<TsiService> logger;
        private readonly HttpClient httpClient;
        private ITimeSeriesInsightsClient? client;

        public TsiService(IOptionsMonitor<TsiOptions> options,
                          IHttpClientFactory factory,
                          IAzureServiceTokenProvider tokenProvider,
                          ITelemetry telemetry,
                          IDateTimeProvider dateTimeProvider,
                          ILogger<TsiService> logger)
        {
            this.options = options;
            this.tokenProvider = tokenProvider;
            this.telemetry = telemetry;
            this.dateTimeProvider = dateTimeProvider;
            this.logger = logger;
            this.httpClient = factory.CreateClient(HttpClientNames.TimeSeriesInsights);
        }

        public async Task InitAsync()
        {
            if (client != null) { return; }
            this.client = await GetTimeSeriesInsightsClientAsync();
        }

        private DateTimeOffset StartTelemetry()
        {
            return dateTimeProvider.UtcNow;
        }

        private void StopTelemetry(string queryType, DateTimeOffset startTime)
        {
            var milliseconds = (dateTimeProvider.UtcNow - startTime).TotalMilliseconds;
            telemetry.TrackMetricValue("AmphoraData.TimeSeriesInsights.Latency", queryType, milliseconds);
        }

        public async Task<IList<TimeSeriesInstance>> GetInstancesAsync()
        {
            await InitAsync();
            var instances = new List<TimeSeriesInstance>();
            string? continuationToken = null;
            GetInstancesPage page;
            do
            {
                // this method will return everything in ONE set (one graphed line). to split, call it twice
                page = await client.GetInstancesPagedAsync(continuationToken);
                continuationToken = page.ContinuationToken;
                instances.AddRange(page.Instances);
            }
            while (continuationToken != null);
            return instances;
        }

        public async Task<QueryResultPage> RunQueryAsync(QueryRequest query, string? continuationToken = null)
        {
            var start = StartTelemetry();
            await InitAsync();
            var queryResponse = await client.ExecuteQueryPagedAsync(query, continuationToken);
            var queryType = "unknown";
            if (query.AggregateSeries != null)
            {
                queryType = nameof(query.AggregateSeries);
            }
            else if (query.GetEvents != null)
            {
                queryType = nameof(query.GetEvents);
            }
            else if (query.GetSeries != null)
            {
                queryType = nameof(query.GetSeries);
            }

            StopTelemetry(queryType, start);
            return queryResponse;
        }

        public async Task<QueryResultPage> RunGetEventsAsync(IList<object> ids,
                                                            DateTimeRange span,
                                                            IList<string>? properties = null)
        {
            var start = StartTelemetry();
            await InitAsync();
            string continuationToken;
            QueryResultPage queryResponse;
            do
            {
                // this method will return everything in ONE set (one graphed line). to split, call it twice
                queryResponse = await client.ExecuteQueryPagedAsync(
                   new QueryRequest(
                       getEvents: new Microsoft.Azure.TimeSeriesInsights.Models.GetEvents(
                           timeSeriesId: ids,
                           searchSpan: span,
                           // projectedProperties: properties,
                           filter: null)));

                continuationToken = queryResponse.ContinuationToken;
            }
            while (continuationToken != null);
            StopTelemetry("GetEvents", start);
            return queryResponse;
        }

        public async Task<QueryResultPage> RunGetAggregateAsync(IList<object> ids,
                                                                IDictionary<string, Variable> variables,
                                                                DateTimeRange span,
                                                                TimeSpan? interval = null,
                                                                IList<string>? projections = null)
        {
            var start = StartTelemetry();
            await InitAsync();
            interval ??= TimeSpan.FromDays(365);
            string continuationToken;
            QueryResultPage queryResponse;
            do
            {
                // this method will return everything in ONE set (one graphed line). to split, call it twice
                queryResponse = await client.ExecuteQueryPagedAsync(
                   new QueryRequest(
                       aggregateSeries: new Microsoft.Azure.TimeSeriesInsights.Models.AggregateSeries(
                           timeSeriesId: ids,
                           searchSpan: span,
                           interval: interval.Value,
                           filter: null,
                           projectedVariables: projections,
                           inlineVariables: variables)));

                continuationToken = queryResponse.ContinuationToken;
            }
            while (continuationToken != null);
            StopTelemetry("GetAggregate", start);
            return queryResponse;
        }

        public async Task<QueryResultPage> RunGetSeriesAsync(IList<object> ids,
                                                             IDictionary<string, Variable> variables,
                                                             DateTimeRange span,
                                                             IList<string>? projections = null)
        {
            var start = StartTelemetry();
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
            StopTelemetry("GetSeries", start);
            return queryResponse;
        }

        private async Task<ITimeSeriesInsightsClient> GetTimeSeriesInsightsClientAsync()
        {
            logger.LogInformation($"Start {nameof(GetTimeSeriesInsightsClientAsync)}");
            var token = await tokenProvider.GetAccessTokenAsync(Resource);
            var serviceClientCredentials = new TokenCredentials(token);

            var timeSeriesInsightsClient = new TimeSeriesInsightsClient(serviceClientCredentials, this.httpClient, false)
            {
                EnvironmentFqdn = options.CurrentValue.DataAccessFqdn
            };
            logger.LogInformation($"Finish {nameof(GetTimeSeriesInsightsClientAsync)}");
            return timeSeriesInsightsClient;
        }
    }
}