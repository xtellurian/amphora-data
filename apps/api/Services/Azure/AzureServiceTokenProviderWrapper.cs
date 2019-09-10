using System.Threading.Tasks;
using Amphora.Api.Contracts;

namespace Amphora.Api.Services.Azure
{
    public class AzureServiceTokenProviderWrapper: IAzureServiceTokenProvider
    {
        private readonly Microsoft.Azure.Services.AppAuthentication.AzureServiceTokenProvider az;

        public AzureServiceTokenProviderWrapper()
        {
            this.az = new Microsoft.Azure.Services.AppAuthentication.AzureServiceTokenProvider();
        }
        public async Task<string> GetAccessTokenAsync(string resource, string tenantId = null)
        {
            return await az.GetAccessTokenAsync(resource, tenantId);
        }
    }
}