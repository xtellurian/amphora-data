using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.AzureMaps;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Services.Azure
{
    public class InMemoryMapOptions
    {
        public string SubscriptionKey { get; set; }
    }

    public class InMemoryMapService : IMapService
    {
        private readonly IOptionsMonitor<InMemoryMapOptions> options;

        public InMemoryMapService(IOptionsMonitor<InMemoryMapOptions> options)
        {
            this.options = options;
        }

        public Task<FuzzySearchResponse> FuzzySearchAsync(string query)
        {
            return Task<FuzzySearchResponse>.Factory.StartNew(() =>
            {
                return new FuzzySearchResponse
                {
                    Summary = new Summary(),
                    Results = new System.Collections.Generic.List<Result>()
                    {
                        new Result
                        {
                            Address = new Address { FreeformAddress = "A Fake Address" },
                            Id = "fake"
                        }
                    }
                };
            });
        }

        public Task<byte[]> GetStaticMapImageAsync(GeoLocation location, int height, int width)
        {
            return Task.FromResult<byte[]>(null);
        }

        public Task<string> GetSubscriptionKeyAsync()
        {
            return Task.FromResult(this.options.CurrentValue.SubscriptionKey);
        }
    }
}