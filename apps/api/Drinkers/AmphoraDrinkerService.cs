using System.IO;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;

namespace Amphora.Api.Drinkers
{
    public class AmphoraDrinkerService : IAmphoraDrinkerService
    {
        private readonly IAmphoraEntityStore<AmphoraModel> amphoraStore;
        private readonly IBinaryAmphoraDrinker binaryDrinker;

        public AmphoraDrinkerService(IAmphoraEntityStore<AmphoraModel> amphoraStore,
            IBinaryAmphoraDrinker binaryDrinker)
        {
            this.amphoraStore = amphoraStore;
            this.binaryDrinker = binaryDrinker;
        }
        public async Task<Stream> DrinkBinary(string amphoraId)
        {
            var amphora = amphoraStore.Get(amphoraId);
            switch(amphora?.Class)
            {
                case AmphoraClass.Binary:
                    return await binaryDrinker.Drink(amphora);
                case AmphoraClass.TimeSeries:
                    throw new System.NotImplementedException("Cannot Drink binary from a time series");
            }
            return null;
        }
    }
}