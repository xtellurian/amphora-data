using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Common.Models.AzureMaps;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Amphora.Api.Services.Azure
{
    public class AzureMapService : IMapService
    {
        private readonly HttpClient client;
        private readonly IAzureServiceTokenProvider tokenProvider;
        private readonly ILogger<AzureMapService> logger;
        private readonly string subscriptionKey;
        private const string apiVersion = "1.0";
        private const string countrySet = "AU";


        public AzureMapService(IHttpClientFactory factory,
            IAzureServiceTokenProvider tokenProvider,
            IOptionsMonitor<AzureMapsOptions> options,
            ILogger<AzureMapService> logger)
        {
            this.client = factory.CreateClient("azure-maps");
            client.BaseAddress = new System.Uri("https://atlas.microsoft.com");
            if(options.CurrentValue.Key != null)
            {
                this.subscriptionKey = options.CurrentValue.Key;
                isInit = true;
            }
            else
            {
                client.DefaultRequestHeaders.Add("x-ms-client-id", options.CurrentValue.Key);
            }
            this.tokenProvider = tokenProvider;
            this.logger = logger;
        }

        private bool isInit;
        private async Task InitAsync()
        {
            if (isInit) return;
            var token = await tokenProvider.GetAccessTokenAsync("https://atlas.microsoft.com/");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            isInit = true;
        }

        public async Task<FuzzySearchResponse> FuzzySearchAsync(string query)
        {
            if (query == null) return new FuzzySearchResponse() { Results = new List<Result>() };
            await InitAsync();
            try
            {
                var queryString = $"api-version={apiVersion}&countrySet={countrySet}&query={query}";
                if(subscriptionKey != null)
                {
                    queryString += $"&subscription-key={subscriptionKey}";
                }
                var response = await client.GetAsync($"search/fuzzy/json?{queryString}");

                var content = await response.Content.ReadAsStringAsync();
                var o = JsonConvert.DeserializeObject<FuzzySearchResponse>(content);
                return o;
            }
            catch (Exception ex)
            {
                logger.LogError("Fuzzy Location Search error", ex);
                this.isInit = false;
            }
            return new FuzzySearchResponse { Results = new List<Result>() };
        }

    }
}