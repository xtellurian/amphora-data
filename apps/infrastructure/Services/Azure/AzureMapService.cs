using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.AzureMaps;
using Amphora.Infrastructure.Models;
using Amphora.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Amphora.Api.Services.Azure
{
    public class AzureMapService : IMapService
    {
        private GeoLocation defaultPosition = new GeoLocation(133.77, -25.27); // australia center
        private readonly HttpClient client;
        private readonly IAzureServiceTokenProvider tokenProvider;
        private readonly ILogger<AzureMapService> logger;
        private readonly string? subscriptionKey;
        private const string ApiVersion = "1.0";
        private const string DefaultCountrySet = "AU,US";
        private string QueryString() => $"subscription-key={subscriptionKey}&api-version={ApiVersion}";

        public AzureMapService(IHttpClientFactory factory,
            IAzureServiceTokenProvider tokenProvider,
            IOptionsMonitor<AzureMapsOptions> options,
            ILogger<AzureMapService> logger)
        {
            this.client = factory.CreateClient(HttpClientNames.AzureMaps);
            client.BaseAddress = new System.Uri("https://atlas.microsoft.com");
            if (options.CurrentValue.Key != null)
            {
                this.subscriptionKey = options.CurrentValue.Key;
                isInit = true;
                logger.LogInformation("Using key based authentication");
            }
            else
            {
                client.DefaultRequestHeaders.Add("x-ms-client-id", options.CurrentValue.Key);
                logger.LogWarning("Using Active Directiory based authentication");
            }

            this.tokenProvider = tokenProvider;
            this.logger = logger;
        }

        private bool isInit;
        private async Task InitAsync()
        {
            if (isInit) { return; } // if we're not using AD auth
            var token = await tokenProvider.GetAccessTokenAsync("https://atlas.microsoft.com/");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            isInit = true;
        }

        public Task<string?> GetSubscriptionKeyAsync()
        {
            return Task.FromResult(this.subscriptionKey);
        }

        public async Task<IpAddressToLocationResult> GetCountryFromIp(string ipAddress)
        {
            await InitAsync();
            var queryString = QueryString();
            queryString += $"&ip={ipAddress}";
            var response = await client.GetAsync($"geolocation/ip/json?{queryString}");
            var content = await response.Content.ReadAsStringAsync();
            var o = JsonConvert.DeserializeObject<IpAddressToLocationResult>(content);
            return o;
        }

        public async Task<GeoLocation> GetPositionFromIp(IPAddress ipAddress)
        {
            return await this.GetPositionFromIp(ipAddress.ToString());
        }

        public async Task<GeoLocation> GetPositionFromIp(string ipAddress)
        {
            var region = await GetCountryFromIp(ipAddress);
            if (region.CountryRegion?.IsoCode != null)
            {
                var fuzzy = await FuzzySearchAsync(region.CountryRegion.IsoCode);
                var first = fuzzy.Results.FirstOrDefault();
                return first.Position?.Lat == null ? defaultPosition : new GeoLocation(first.Position.Lon, first.Position.Lat);
            }
            else
            {
                return defaultPosition;
            }
        }

        public async Task<FuzzySearchResponse> FuzzySearchAsync(string query, string? countrySet = null)
        {
            if (query == null) { return new FuzzySearchResponse() { Results = new List<Result>() }; }
            await InitAsync();
            try
            {
                var queryString = QueryString();
                queryString += $"&query={query}&countrySet={countrySet ?? DefaultCountrySet}";
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

        public async Task<byte[]> GetStaticMapImageAsync(GeoLocation location, int height = 512, int width = 512)
        {
            if (location.Coordinates.Length == 0)
            {
                throw new ArgumentException("Geolocation has null lon lat");
            }

            await InitAsync();
            var lon = location.Lon();
            var lat = location.Lat();
            var queryString = QueryString();
            queryString += $"&zoom=6";
            queryString += $"&height={height}&width={width}";
            queryString += $"&center={lon},{lat}";
            queryString += $"&pins=default|coBC312A|lc000000||'A'{lon} {lat}";

            try
            {
                var response = await client.GetAsync($"map/static/png?{queryString}"); // return type expects PNG
                response.EnsureSuccessStatusCode();
                var bytes = await response.Content.ReadAsByteArrayAsync();
                return bytes;
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to get Static Map Image", ex);
                return new byte[0];
            }
        }
    }
}