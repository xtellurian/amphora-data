using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Microsoft.Extensions.Options;
using Amphora.Api.Options;
using System.Net.Http;
using System.IO;

namespace Amphora.Api.Services
{
    public class TsiService: ITsiService
    {
        private const string resource = "https://api.timeseries.azure.com/";
        private readonly AzureServiceTokenProvider azureServiceTokenProvider;
        private readonly IOptionsMonitor<TsiOptions> options;
        private readonly HttpClient client;

        public TsiService(IOptionsMonitor<TsiOptions> options, IHttpClientFactory clientFactory)
        {
            azureServiceTokenProvider = new AzureServiceTokenProvider();
            this.options = options;
            var client = clientFactory.CreateClient("tsi");
            var fqdn = options.CurrentValue.DataAccessFqdn;
            if( ! fqdn.StartsWith("https"))
            {
                fqdn = $"https://{fqdn}";
            }
            client.BaseAddress = new System.Uri(fqdn);
            this.client = client;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var token = await azureServiceTokenProvider.GetAccessTokenAsync(resource);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return token;
        }

        public string GetDataAccessFqdn()
        {
            if(options.CurrentValue.DataAccessFqdn == null)
            {
                throw new System.ArgumentNullException("TsiOptions.DataAccessFqdn");
            }
            return options.CurrentValue.DataAccessFqdn;
        }

        public async Task<HttpResponseMessage> ProxyQueryAsync(string uri, HttpContent content)
        {
            await GetAccessTokenAsync(); // make sure the token is loaded.
            var response = await client.PostAsync(uri, content);
            return response;
        }
    }
}