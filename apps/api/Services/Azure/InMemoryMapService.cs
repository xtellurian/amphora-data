using System.Net;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.AzureMaps;
using Amphora.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Services.Azure
{
    public class InMemoryMapService : IMapService
    {
        private readonly IOptionsMonitor<AzureMapsOptions> options;

        public InMemoryMapService(IOptionsMonitor<AzureMapsOptions> options)
        {
            this.options = options;
        }

        public Task<FuzzySearchResponse> FuzzySearchAsync(string query, string countrySet = null)
        {
            return Task<FuzzySearchResponse>.Run(() =>
            {
                return new FuzzySearchResponse
                {
                    Summary = new Summary
                    {
                        Query = query,
                        NumResults = 1
                    },
                    Results = new System.Collections.Generic.List<Result>()
                    {
                        new Result
                        {
                            Address = new Address { FreeformAddress = "Somewhere in Sydney" },
                            Id = "fake-sydney",
                            Position = new Position
                            {
                                Lat = -33.8688,
                                Lon = 151.2093
                            }
                        },
                        new Result
                        {
                            Address = new Address { FreeformAddress = "Somewhere in Melbourne" },
                            Id = "fake-melbourne",
                            Position = new Position
                            {
                                Lat = -37.8136,
                                Lon = 144.9631
                            }
                        }
                    }
                };
            });
        }

        public Task<GeoLocation> GetPositionFromIp(string ipAddress)
        {
            return Task.FromResult(new GeoLocation(133.77, -25.27)); // aus center
        }

        public Task<GeoLocation> GetPositionFromIp(IPAddress ipAddress)
        {
            return this.GetPositionFromIp(ipAddress.ToString());
        }

        public Task<byte[]> GetStaticMapImageAsync(GeoLocation location, int height, int width)
        {
            return Task.FromResult<byte[]>(null);
        }

        public Task<string> GetSubscriptionKeyAsync()
        {
            return Task.FromResult(this.options.CurrentValue.Key);
        }
    }
}