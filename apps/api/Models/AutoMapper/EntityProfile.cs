using Amphora.Api.Models.Dtos;
using Amphora.Common.Models;
using AutoMapper;

namespace Amphora.Api.Models.AutoMapper
{
    public class EntityProfile : Profile
    {
        public EntityProfile()
        {
            CreateMap<EntityDto, Entity>()
                .ForMember(p => p.ttl, o => o.Ignore())
                .ForMember(p => p.IsDeleted, o => o.Ignore())
                .ReverseMap();

        }
    }
}
