using AutoMapper;
using Amphora.Common.Models;
using Amphora.Api.Models;

namespace api.Models.AutoMapper
{
    public class SchemaProfile : Profile
    {
        public SchemaProfile()
        {
            CreateMap<Schema, SchemaTableEntity>()
                .ForMember(m => m.RowKey, cfg => cfg.Ignore())
                .ForMember(m => m.PartitionKey, cfg => cfg.Ignore())
                .ForMember(m => m.ETag, cfg => cfg.Ignore())
                .ForMember(m => m.Timestamp, cfg => cfg.Ignore())
                .ForMember(dest => dest.RowKey, opt =>
                {
                    opt.MapFrom((src) => src.Id);
                })
                .ReverseMap();
        }
    }

}