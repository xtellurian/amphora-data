using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos;
using Amphora.Api.Models.Dtos.Feeds;
using Amphora.Api.Models.Feeds;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    [Route("api/feeds")]
    [OpenApiTag("Feeds")]
    public class FeedController : EntityController
    {
        private readonly IFeedAggregator feedAggregator;
        private readonly IMapper mapper;

        public FeedController(IFeedAggregator feedAggregator, IMapper mapper)
        {
            this.feedAggregator = feedAggregator;
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets the feed for the logged in user.
        /// </summary>
        /// <returns>A Feed object.</returns>
        [HttpGet("v1")]
        [CommonAuthorize]
        [Produces(typeof(CollectionResponse<FeedItem>))]
        [ProducesBadRequest]
        [ValidateModel]
        public async Task<IActionResult> GetFeed()
        {
            var feedRes = await feedAggregator.GetFeedAsync(User);
            if (feedRes.Succeeded)
            {
                var feedItems = mapper.Map<List<FeedItem>>(feedRes.Entity);
                var dto = new CollectionResponse<FeedItem>(feedItems);
                return Ok(dto);
            }
            else
            {
                return Handle(feedRes);
            }
        }
    }
}