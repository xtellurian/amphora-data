using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Models.Search;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Contracts
{
    public interface IDiscoverService<T> where T : ISearchable
    {
        Task<EntitySearchResult<T>> FindAsync(string searchTerm,
                                                  GeoLocation location = null,
                                                  double? distance = null,
                                                  int? skip = 0,
                                                  int? top = 15,
                                                  IEnumerable<string> labels = null);
    }
}