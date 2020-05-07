using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.AzureMaps;

namespace Amphora.Api.Services.Azure
{
    public class InMemoryMapService : IMapService
    {
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
    }
}