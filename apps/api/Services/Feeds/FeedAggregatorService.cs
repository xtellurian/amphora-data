using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Feeds;
using Amphora.Common.Contracts;
using Amphora.Common.Models;

namespace Amphora.Api.Services.Feeds
{
    public class FeedAggregatorService : IFeedAggregator
    {
        private readonly IUserDataService userDataService;
        private readonly IAmphoraFeedService amphoraFeed;

        public FeedAggregatorService(IUserDataService userDataService, IAmphoraFeedService amphoraFeed)
        {
            this.userDataService = userDataService;
            this.amphoraFeed = amphoraFeed;
        }

        public async Task<EntityOperationResult<IEnumerable<IFeedEvent>>> GetFeedAsync(ClaimsPrincipal principal)
        {
            var userRead = await userDataService.ReadAsync(principal);
            if (userRead.Failed)
            {
                return new EntityOperationResult<IEnumerable<IFeedEvent>>("Unknown User");
            }

            var posts = await amphoraFeed.GetPostsAsync(userRead.Entity.Organisation);
            return new EntityOperationResult<IEnumerable<IFeedEvent>>(userRead.Entity, posts);
        }
    }
}