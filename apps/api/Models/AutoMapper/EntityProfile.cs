using Amphora.Common.Models;
using AutoMapper;

namespace Amphora.Api.Models.AutoMapper
{
    public class EntityProfile : Profile
    {
        public EntityProfile()
        {
            CreateMap<Amphora.Api.Models.Dtos.Entity, EntityBase>()
                .ForMember(p => p.ttl, o => o.Ignore())
                .ForMember(p => p.IsDeleted, o => o.Ignore())
                .ForMember(p => p.LastModified, o => o.Ignore())
                .ReverseMap();
        }
    }
}
