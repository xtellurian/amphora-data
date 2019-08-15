using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Microsoft.Extensions.Options;
using Amphora.Api.Options;
using System.Net.Http;
using System.IO;
using Microsoft.Extensions.Logging;
using TimeSeriesInsightsClient;
using TimeSeriesInsightsClient.Queries;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Amphora.Api.Services
{
    public class RealTsiService : ITsiService
    {
        private const string resource = "https://api.timeseries.azure.com/";
        private readonly IOptionsMonitor<TsiOptions> options;
        private readonly ILogger<RealTsiService> logger;
        private readonly IAzureServiceTokenProvider tokenProvider;
        private readonly TsiClient client;

        public RealTsiService(IOptionsMonitor<TsiOptions> options,
            IHttpClientFactory clientFactory,
            ILogger<RealTsiService> logger,
            IAzureServiceTokenProvider tokenProvider)
        {
            this.options = options;
            this.logger = logger;
            this.tokenProvider = tokenProvider;
            var client = clientFactory.CreateClient("tsi");
            var fqdn = options.CurrentValue.DataAccessFqdn;
            if (string.IsNullOrEmpty(fqdn))
            {
                logger.LogWarning("FQDN missing");
            }
            else
            {

                if (!fqdn.StartsWith("https"))
                {
                    fqdn = $"https://{fqdn}";
                }
                client.BaseAddress = new System.Uri(fqdn);

                this.client = new TsiClient(client);
            }
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var token = await tokenProvider.GetAccessTokenAsync(resource);
            this.client.HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return token;
        }

        public string GetDataAccessFqdn()
        {
            if (options.CurrentValue.DataAccessFqdn == null)
            {
                throw new System.ArgumentNullException("TsiOptions.DataAccessFqdn");
            }
            return options.CurrentValue.DataAccessFqdn;
        }

        public async Task<HttpResponseMessage> ProxyQueryAsync(string uri, HttpContent content)
        {
            await GetAccessTokenAsync(); // make sure the token is loaded.
            var response = await client.HttpClient.PostAsync(uri, content);
            return response;
        }

        public async Task<QueryResponse> WeeklyAverageAsync(string id, string property, DateTime start, DateTime end )
        {
            await GetAccessTokenAsync(); // make sure the token is loaded.
            var variableName = "Avg";
            var query = new Query
            {
                AggregateSeries = new AggregateSeries
                {
                    TimeSeriesId = new List<string>{id},
                    Interval = "P7D",
                    SearchSpan = new SearchSpan
                    {
                        From = start,
                        To = end
                    },
                    InlineVariables = new Dictionary<string, InlineVariable>
                    {
                        {variableName, new InlineVariable
                        {
                            Kind= "numeric",
                            Value = new Aggregation { Tsx = $"$event.{property}"},
                            Aggregation = new Aggregation { Tsx = "avg($value)"}
                        }}
                    },
                    ProjectedVariables = new List<string> {variableName}
                }
            };
            var response = await client.QueryAsync(query);
           
            return response;
        }
    }
}