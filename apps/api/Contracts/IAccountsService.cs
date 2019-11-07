using System.Threading.Tasks;

namespace Amphora.Api.Contracts
{
    public interface IAccountsService
    {
        Task PopulateDebitsAsync();
    }
}