using Amphora.Api.Models.Search;
using Amphora.Common.Models;
using AutoMapper;
using Microsoft.Azure.Search.Models;

namespace Amphora.Api.Models.AutoMapper
{
    public class AzureSearchProfile : Profile
    {
        public AzureSearchProfile()
        {
            // CreateMap<DocumentSearchResult<AmphoraModel>, EntitySearchResult<AmphoraModel>>();
            CreateMap<DocumentSearchResult<AmphoraModel>, EntitySearchResult<AmphoraModel>>();

            CreateMap<Microsoft.Azure.Search.Models.SearchResult<AmphoraModel>, Search.SearchResult<AmphoraModel>>()
            .ForMember( a => a.Entity, opt => opt.MapFrom(src => src.Document));
            // .ForMember(p => p., o => o.Ignore())
        }
    }
}
