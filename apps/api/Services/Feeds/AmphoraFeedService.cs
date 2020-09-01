using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Feeds;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Purchases;
using Microsoft.EntityFrameworkCore;

namespace Amphora.Api.Services.Feeds
{
    public class AmphoraFeedService : IAmphoraFeedService
    {
        private readonly IEntityStore<AmphoraModel> amphoraStore;
        private readonly IEntityStore<PurchaseModel> purchaseStore;

        public AmphoraFeedService(IEntityStore<AmphoraModel> amphoraStore, IEntityStore<PurchaseModel> purchaseStore)
        {
            this.amphoraStore = amphoraStore;
            this.purchaseStore = purchaseStore;
        }

        public async Task<IEnumerable<IPost>> GetPostsAsync(OrganisationModel org, int take = 10)
        {
            // get latest created
            var posts = new List<IPost>();
            posts.AddRange(await GetAmphoraCreatedPosts(org, take / 2));
            posts.AddRange(await GetAmphoraPurchasedPosts(org, take / 2));
            posts.Sort((x, y) => y.Timestamp.CompareTo(x.Timestamp)); // this should order descending timestamp
            return posts;
        }

        private async Task<List<AmphoraPurchasedPost>> GetAmphoraPurchasedPosts(OrganisationModel org, int take)
        {
            var recentlyPurchased = await purchaseStore.Query(p => p.PurchasedByOrganisationId == org.Id)
                .OrderByDescending(_ => _.CreatedDate)
                .Take(take)
                .ToListAsync();

            var posts = new List<AmphoraPurchasedPost>();
            foreach (var p in recentlyPurchased)
            {
                posts.Add(new AmphoraPurchasedPost(p));
            }

            return posts;
        }

        private async Task<List<AmphoraCreatedPost>> GetAmphoraCreatedPosts(OrganisationModel org, int take)
        {
            var recentlyCreatedAmphora = await amphoraStore.Query(a => a.OrganisationId == org.Id)
                .OrderByDescending(_ => _.CreatedDate)
                .Take(take)
                .ToListAsync();

            var posts = new List<AmphoraCreatedPost>();
            foreach (var a in recentlyCreatedAmphora)
            {
                posts.Add(new AmphoraCreatedPost(a));
            }

            return posts;
        }
    }
}