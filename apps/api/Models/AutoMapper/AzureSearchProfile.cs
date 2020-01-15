using Amphora.Api.Models.Search;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.DataRequests;
using AutoMapper;
using Microsoft.Azure.Search.Models;

namespace Amphora.Api.Models.AutoMapper
{
    public class AzureSearchProfile : Profile
    {
        public AzureSearchProfile()
        {
            CreateMap<DocumentSearchResult<AmphoraModel>, EntitySearchResult<AmphoraModel>>();

            CreateMap<Microsoft.Azure.Search.Models.SearchResult<AmphoraModel>, Search.SearchResult<AmphoraModel>>()
            .ForMember(a => a.Entity, opt => opt.MapFrom(src => src.Document));
            // .ForMember(p => p., o => o.Ignore())

            CreateMap<DocumentSearchResult<DataRequestModel>, EntitySearchResult<DataRequestModel>>();

            CreateMap<Microsoft.Azure.Search.Models.SearchResult<DataRequestModel>, Search.SearchResult<DataRequestModel>>()
            .ForMember(a => a.Entity, opt => opt.MapFrom(src => src.Document));
            // .ForMember(p => p., o => o.Ignore())
        }
    }
}
