using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Common.Models;

namespace Amphora.Api.Contracts
{
    public interface IMarketService
    {
        Task<IEnumerable<Amphora.Common.Models.AmphoraModel>> FindAsync(SearchParams searchParams);
    }
}