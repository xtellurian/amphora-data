using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Signals;
using AutoMapper;

namespace Amphora.Api.Models.AutoMapper
{
    public class AmphoraModelProfile : Profile
    {
        public AmphoraModelProfile()
        {
            // this is to extend a base class into its extended version. 
            // Generally only used in the InMemory stores.

            CreateMap<AmphoraModel, AmphoraDto>();

            CreateMap<CreateAmphoraDto, AmphoraModel>()
                .ForMember(o => o.Id, p => p.Ignore())
                .ForMember(o => o.Organisation, p => p.Ignore())
                .ForMember(o => o.OrganisationId, p => p.Ignore())
                .ForMember( o => o.TermsAndConditions, p => p.Ignore())
                .ForMember(o => o.IsPublic, p => p.MapFrom(src => true))
                .ForMember(o => o.Signals, p => p.Ignore())
                .ForMember(o => o.Purchases, p => p.Ignore())
                .ForMember(o => o.ttl, p => p.Ignore())
                .ForMember(o => o.CreatedBy, p => p.Ignore())
                .ForMember(o => o.CreatedById, p => p.Ignore())
                .ForMember(o => o.CreatedDate, p => p.Ignore())
                .ForMember(o => o.GeoLocation, p => p.MapFrom(src => 
                    src.Lat.HasValue && src.Lon.HasValue ? new GeoLocation(src.Lon.Value, src.Lat.Value): null));

            CreateMap<AmphoraExtendedDto, AmphoraModel>()
            .ForMember(o => o.Id, p => p.Ignore())
            .ForMember(o => o.Organisation, p => p.Ignore())
            .ForMember(o => o.TermsAndConditions, p => p.Ignore())
            .ForMember(o => o.IsPublic, p => p.MapFrom(src => true))
            .ForMember(o => o.Signals, p => p.Ignore())
            .ForMember(o => o.Purchases, p => p.Ignore())
            .ForMember(o => o.ttl, p => p.Ignore())
            .ForMember(o => o.CreatedBy, p => p.Ignore())
            .ForMember(o => o.CreatedById, p => p.Ignore())
            .ForMember(o => o.CreatedDate, p => p.Ignore())
            .ForMember(o => o.GeoLocation, p => p.MapFrom(src => 
                src.Lat.HasValue && src.Lon.HasValue ? new GeoLocation(src.Lon.Value, src.Lat.Value): null));

            CreateMap<AmphoraModel, AmphoraExtendedDto>()
            .IncludeBase<AmphoraModel, AmphoraDto>()
            .ForMember(o => o.Lat, p => p.MapFrom(src => src.GeoLocation.Lat()))
            .ForMember(o => o.Lon, p => p.MapFrom(src => src.GeoLocation.Lon()));

            CreateMap<SignalModel, SignalDto>();
        }
    }
}
