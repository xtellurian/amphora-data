using System.Collections.Generic;
using Amphora.Api.Models.Dtos;

namespace Amphora.Api.Models.Feeds
{
    public class Feed : CollectionResponse<IPost>
    {
        public Feed(IEnumerable<IPost> items) : base(items)
        {
        }
    }
}