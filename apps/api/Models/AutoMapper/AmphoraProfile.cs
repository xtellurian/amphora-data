using AutoMapper;
using Amphora.Api.Models;

namespace api.Models.AutoMapper
{
    public class AmphoraProfile : Profile
    {
        public AmphoraProfile()
        {
            CreateMap<Amphora.Common.Models.Amphora, AmphoraTableEntity>()
                .ForMember(m => m.RowKey, cfg => cfg.Ignore())
                .ForMember(m => m.PartitionKey, cfg => cfg.Ignore())
                .ForMember(m => m.ETag, cfg => cfg.Ignore())
                .ForMember(m => m.Timestamp, cfg => cfg.Ignore())
                .ForMember(dest => dest.RowKey, opt =>
                {
                    opt.MapFrom((src) => src.Id);
                })
                .ForMember(dest => dest.PartitionKey, opt =>
                {
                    opt.MapFrom((src) => src.OrgId);
                })
                .ReverseMap();
        }
    }

}