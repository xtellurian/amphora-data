using Amphora.Api.Models.Dtos.Feeds;
using Amphora.Api.Models.Feeds;
using AutoMapper;

namespace Amphora.Api.Models.AutoMapper
{
    public class FeedProfile : Profile
    {
        public FeedProfile()
        {
            CreateMap<IFeedEvent, FeedItem>();
        }
    }
}
