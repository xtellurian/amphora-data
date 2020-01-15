using System.Threading.Tasks;

namespace Amphora.Api.Contracts
{
    public interface IAzureSearchInitialiser<T>
    {
        Task CreateIndexAsync();
        Task<bool> TryIndex();
    }
}