using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
namespace Amphora.Api.Services
{
    public class SimpleMarketSearchService
    {
        private readonly IOrgEntityStore<Common.Models.Amphora> amphoraStore;
        private readonly IOrgEntityStore<Tempora> temporaStore;

        public SimpleMarketSearchService(
            IOrgEntityStore<Amphora.Common.Models.Amphora> amphoraStore,
            IOrgEntityStore<Amphora.Common.Models.Tempora> temporaStore )
        {
            this.amphoraStore = amphoraStore;
            this.temporaStore = temporaStore;
        }

        public async Task<IEnumerable<Amphora.Common.Models.MarketEntity>> Search(string term)
        {
            var amphora = await amphoraStore.ListAsync();
            var tempora = await temporaStore.ListAsync();
            throw new NotImplementedException();
        }
    }
}