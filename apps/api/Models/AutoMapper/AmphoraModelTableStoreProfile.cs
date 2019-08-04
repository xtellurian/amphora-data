using AutoMapper;
using Amphora.Common.Models;

namespace api.Models.AutoMapper
{
    public class AmphoraModelTableStoreProfile : Profile
    {
        public AmphoraModelTableStoreProfile()
        {
            CreateMap<AmphoraModel, AmphoraModelTableEntity>();
            CreateMap<AmphoraModelTableEntity, AmphoraModel>();
        }
    }

}