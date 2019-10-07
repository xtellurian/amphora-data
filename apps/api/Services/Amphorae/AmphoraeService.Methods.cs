using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Services.Amphorae
{
    public partial class AmphoraeService
    {
        public async Task<IEnumerable<AmphoraModel>> AmphoraPurchasedBy(IUser user)
        {
            var transactions = await purchaseStore.QueryAsync(t => t.PurchasedByUserId == user.Id);
            var amphorae = await AmphoraStore.QueryAsync(a => transactions.Select(t => t.AmphoraId).Contains(a.Id));
            return amphorae;
        }
    }
}