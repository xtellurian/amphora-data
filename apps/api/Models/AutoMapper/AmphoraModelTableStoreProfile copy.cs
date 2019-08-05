using AutoMapper;
using Amphora.Common.Models;
using Amphora.Api.Models;

namespace api.Models.AutoMapper
{
    public class AmphoraSchemaTableStoreProfile : Profile
    {
        public AmphoraSchemaTableStoreProfile()
        {
            CreateMap<Schema, AmphoraSchemaTableEntity>()
                .ForMember(m => m.RowKey, cfg => cfg.Ignore())
                .ForMember(m => m.PartitionKey, cfg => cfg.Ignore())
                .ForMember(m => m.ETag, cfg => cfg.Ignore())
                .ForMember(m => m.Timestamp, cfg => cfg.Ignore())
                .ReverseMap();
        }
    }

}