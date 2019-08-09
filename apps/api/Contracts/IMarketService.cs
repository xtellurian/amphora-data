using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Models;

namespace Amphora.Api.Contracts
{
    public interface IMarketService
    {
        Task<IEnumerable<MarketEntity>> FindAsync(string term);
    }
}