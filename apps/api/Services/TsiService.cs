using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using System.Threading.Tasks;
using Amphora.Api.Contracts;

namespace Amphora.Api.Services
{
    public class TsiService: ITsiService
    {
        private const string resource = "https://api.timeseries.azure.com/";
        private readonly AzureServiceTokenProvider azureServiceTokenProvider;

        public TsiService()
        {
            azureServiceTokenProvider = new AzureServiceTokenProvider();
            
        }

        public async Task<string> GetAccessTokenAsync()
        {
            return await azureServiceTokenProvider.GetAccessTokenAsync(resource);
        }
    }
}