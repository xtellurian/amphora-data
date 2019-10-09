using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Services.Amphorae
{
    public partial class AmphoraeService
    {
        public async Task<IEnumerable<AmphoraModel>> AmphoraPurchasedBy(ClaimsPrincipal principal, IUser user)
        {
            var currentUser = await userService.ReadUserModelAsync(principal);
            using (logger.BeginScope(new LoggerScope<AmphoraeService>(currentUser)))
            {
                var transactions = await purchaseStore.QueryAsync(t => t.PurchasedByUserId == user.Id);
                var amphorae = await AmphoraStore.QueryAsync(a => transactions.Select(t => t.AmphoraId).Contains(a.Id));
                return amphorae;
            }
        }
    }
}