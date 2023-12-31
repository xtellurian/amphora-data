using System.Threading.Tasks;
using Amphora.Common.Contracts;

namespace Amphora.Common.Services.Azure
{
    public class AzureServiceTokenProviderWrapper : IAzureServiceTokenProvider
    {
        private readonly Microsoft.Azure.Services.AppAuthentication.AzureServiceTokenProvider az;

        public AzureServiceTokenProviderWrapper(string? connectionString = null)
        {
            this.az = new Microsoft.Azure.Services.AppAuthentication.AzureServiceTokenProvider(connectionString);
        }

        public async Task<string> GetAccessTokenAsync(string resource, string? tenantId = null)
        {
            return await az.GetAccessTokenAsync(resource, tenantId);
        }
    }
}