using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Contracts
{
    public interface IMarketService
    {
        ISearchService searchService { get; }

        Task<IEnumerable<AmphoraModel>> FindAsync(string searchTerm,
                                                  GeoLocation location = null,
                                                  double? distance = null,
                                                  int? skip = 0,
                                                  int? top = 15);
    }
}