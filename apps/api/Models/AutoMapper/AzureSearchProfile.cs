using Amphora.Api.Models.Search;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.DataRequests;
using Amphora.Common.Models.Organisations;
using AutoMapper;
using Microsoft.Azure.Search.Models;

namespace Amphora.Api.Models.AutoMapper
{
    public class AzureSearchProfile : Profile
    {
        public AzureSearchProfile()
        {
            CreateSearchMap<AmphoraModel>();
            CreateSearchMap<DataRequestModel>();
            CreateSearchMap<OrganisationModel>();
        }

        private void CreateSearchMap<T>()
        {
            CreateMap<DocumentSearchResult<T>, EntitySearchResult<T>>();
            CreateMap<Microsoft.Azure.Search.Models.SearchResult<T>, Search.SearchResult<T>>()
                .ForMember(a => a.Entity, opt => opt.MapFrom(src => src.Document));
        }
    }
}
