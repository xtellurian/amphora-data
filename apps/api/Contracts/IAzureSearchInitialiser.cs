using System.Threading.Tasks;

namespace Amphora.Api.Contracts
{
    public interface IAzureSearchInitialiser
    {
        Task CreateAmphoraIndexAsync();
        Task<bool> TryIndex();
    }
}