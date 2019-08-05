using System.IO;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;

namespace Amphora.Api.Drinkers
{
    public class AmphoraDrinkerService : IAmphoraDrinkerService
    {
        private readonly IEntityStore<Amphora.Common.Models.Amphora> amphoraStore;
        private readonly IBinaryAmphoraDrinker binaryDrinker;

        public AmphoraDrinkerService(IEntityStore<Amphora.Common.Models.Amphora> amphoraStore,
            IBinaryAmphoraDrinker binaryDrinker)
        {
            this.amphoraStore = amphoraStore;
            this.binaryDrinker = binaryDrinker;
        }
        public async Task<Stream> DrinkBinary(string amphoraId)
        {
            var amphora = amphoraStore.Get(amphoraId);

            return await binaryDrinker.Drink(amphora);

        }
    }
}