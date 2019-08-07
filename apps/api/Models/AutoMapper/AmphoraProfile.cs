using AutoMapper;
using Amphora.Api.Models;
using Amphora.Api.ViewModels;

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
                .ReverseMap();

            CreateMap<Amphora.Common.Models.Amphora, AmphoraViewModel>()
                .ReverseMap();
        }
    }

}