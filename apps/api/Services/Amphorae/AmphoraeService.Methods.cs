using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Logging;

namespace Amphora.Api.Services.Amphorae
{
    public partial class AmphoraeService
    {
        public async Task<IQueryable<AmphoraModel>> AmphoraPurchasedBy(ClaimsPrincipal principal, IUser user)
        {
            var currentUser = await userService.ReadUserModelAsync(principal);
            using (logger.BeginScope(new LoggerScope<AmphoraeService>(currentUser)))
            {
                var transactions = await purchaseStore.QueryAsync(t => t.PurchasedByUserId == user.Id);
                var amphorae = AmphoraStore.Query(a => transactions.Select(t => t.AmphoraId).Contains(a.Id));
                return amphorae;
            }
        }
    }
}