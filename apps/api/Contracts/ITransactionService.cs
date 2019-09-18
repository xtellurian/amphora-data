using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Transactions;

namespace Amphora.Api.Contracts
{
    public interface ITransactionService
    {
        Task<EntityOperationResult<TransactionModel>> PurchaseAmphora(ClaimsPrincipal principal, AmphoraModel amphora);
    }
}