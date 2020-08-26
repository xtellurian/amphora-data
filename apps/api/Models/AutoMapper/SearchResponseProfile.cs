using System.Linq;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Api.Models.Dtos.Search;
using Amphora.Api.Models.Search;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using AutoMapper;

namespace Amphora.Api.Models.AutoMapper
{
    public class SearchResponseProfile : Profile
    {
        public SearchResponseProfile()
        {
            CreateMap<EntitySearchResult<AmphoraModel>, SearchResponse<BasicAmphora>>()
                .ForMember(_ => _.Items, p => p.MapFrom(src => src.Results.Select(r => r.Entity)))
                .ForMember(_ => _.Message, p => p.Ignore());

            CreateMap<EntitySearchResult<OrganisationModel>, SearchResponse<Organisation>>()
                .ForMember(_ => _.Items, p => p.MapFrom(src => src.Results.Select(r => r.Entity)))
                .ForMember(_ => _.Message, p => p.Ignore());
        }
    }
}
