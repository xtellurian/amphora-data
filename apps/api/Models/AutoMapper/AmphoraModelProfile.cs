using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using AutoMapper;

namespace Amphora.Api.Models.AutoMapper
{
    public class AmphoraModelProfile : Profile
    {
        public AmphoraModelProfile()
        {
            // this is to extend a base class into its extended version. 
            // Generally only used in the InMemory stores.

            // dto mappings
            CreateMap<AmphoraModel, AmphoraModelDto>();

            CreateMap<AmphoraExtendedDto, AmphoraModel>()
            .ForMember(o => o.Id, p => p.Ignore())
            .ForMember(o => o.Organisation, p => p.Ignore())
            .ForMember(o => o.IsPublic, p => p.MapFrom(src => true))
            .ForMember(o => o.DomainId, p => p.Ignore())
            .ForMember(o => o.Transactions, p => p.Ignore())
            .ForMember(o => o.ttl, p => p.Ignore())
            .ForMember(o => o.CreatedBy, p => p.Ignore())
            .ForMember(o => o.CreatedById, p => p.Ignore())
            .ForMember(o => o.CreatedDate, p => p.Ignore())
            .ForMember(o => o.GeoLocation, p => p.MapFrom(src => 
                src.Lat.HasValue && src.Lon.HasValue ? new GeoLocation(src.Lon.Value, src.Lat.Value): null));

            CreateMap<AmphoraModel, AmphoraExtendedDto>()
            .IncludeBase<AmphoraModel, AmphoraModelDto>()
            .ForMember(o => o.Lat, p => p.MapFrom(src => src.GeoLocation.Lat()))
            .ForMember(o => o.Lon, p => p.MapFrom(src => src.GeoLocation.Lon()));

        }
    }
}
