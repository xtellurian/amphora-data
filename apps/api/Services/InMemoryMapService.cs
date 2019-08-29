using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.AzureMaps;

namespace Amphora.Api.Services
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
                            Address = new Address { FreeformAddress = "A Fake Address "},
                            Id = "fake"
                        }
                    }
                };
            });
        }
    }
}