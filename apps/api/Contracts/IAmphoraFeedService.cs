using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Models.Feeds;
using Amphora.Common.Models.Organisations;

namespace Amphora.Api.Contracts
{
    public interface IAmphoraFeedService
    {
        Task<IEnumerable<IPost>> GetPostsAsync(OrganisationModel org, int take = 10);
    }
}