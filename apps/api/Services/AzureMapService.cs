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

namespace Amphora.Api.Services
{
    public class AzureMapService : IMapService
    {
        private readonly HttpClient client;
        private readonly IAzureServiceTokenProvider tokenProvider;
        private readonly ILogger<AzureMapService> logger;
        private const string apiVersion = "1.0";

        public AzureMapService(IHttpClientFactory factory,
            IAzureServiceTokenProvider tokenProvider,
            IOptionsMonitor<AzureMapsOptions> options,
            ILogger<AzureMapService> logger)
        {
            this.client = factory.CreateClient("azure-maps");
            client.BaseAddress = new System.Uri("https://atlas.microsoft.com");
            client.DefaultRequestHeaders.Add("x-ms-client-id", options.CurrentValue.AzureMapsClientId);
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
                var queryString = $"api-version={apiVersion}&query={query}";
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