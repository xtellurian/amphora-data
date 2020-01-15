using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Models.Search;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Contracts
{
    public interface IMarketService
    {
        ISearchService SearchService { get; }
        Task<EntitySearchResult<AmphoraModel>> FindAsync(string searchTerm,
                                                  GeoLocation location = null,
                                                  double? distance = null,
                                                  int? skip = 0,
                                                  int? top = 15,
                                                  IEnumerable<string> labels = null);
    }
}