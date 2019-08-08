using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Microsoft.Extensions.Options;
using Amphora.Api.Options;
using System.Net.Http;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services
{
    public class RealTsiService : ITsiService
    {
        private const string resource = "https://api.timeseries.azure.com/";
        private readonly IOptionsMonitor<TsiOptions> options;
        private readonly IAzureServiceTokenProvider tokenProvider;
        public HttpClient Client { get; }

        public RealTsiService(IOptionsMonitor<TsiOptions> options,
            IHttpClientFactory clientFactory,
            ILogger<RealTsiService> logger,
            IAzureServiceTokenProvider tokenProvider)
        {
            this.options = options;
            this.tokenProvider = tokenProvider;
            this.Client = clientFactory.CreateClient("tsi");
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
                this.Client.BaseAddress = new System.Uri(fqdn);
            }
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var token = await tokenProvider.GetAccessTokenAsync(resource);
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
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
            var response = await Client.PostAsync(uri, content);
            return response;
        }
    }
}