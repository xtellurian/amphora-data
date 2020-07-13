using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Logging;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Amphorae
{
    public partial class AmphoraeService
    {
        public async Task<IQueryable<AmphoraModel>> AmphoraPurchasedBy(ClaimsPrincipal principal, IUser user)
        {
            using (logger.BeginScope(new LoggerScope<AmphoraeService>(principal)))
            {
                var transactions = await purchaseStore.QueryAsync(t => t.PurchasedByUserId == user.Id, 0, 500);
                if (transactions.Count() > 350)
                {
                    logger.LogWarning("Transactions has more than 350!");
                }

                var amphorae = AmphoraStore.Query(a => transactions.Select(t => t.AmphoraId).Contains(a.Id));
                return amphorae;
            }
        }
    }
}