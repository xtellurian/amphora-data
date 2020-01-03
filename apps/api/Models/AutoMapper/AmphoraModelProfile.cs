using System.Linq;
using Amphora.Api.Models.Dtos;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Signals;
using AutoMapper;

namespace Amphora.Api.Models.AutoMapper
{
    public class AmphoraModelProfile : Profile
    {
        public AmphoraModelProfile()
        {

            CreateMap<AmphoraModel, AmphoraDto>()
                .IncludeBase<Entity, EntityDto>();

            CreateMap<AmphoraModel, EditAmphora>()
                .ForMember(o => o.Labels, p => p.MapFrom(src => string.Join(',', src.Labels.Select(_ => _.Name))))
                .ForMember(o => o.Lat, p => p.MapFrom(src => src.GeoLocation.Lat()))
                .ForMember(o => o.Lon, p => p.MapFrom(src => src.GeoLocation.Lon()));

            CreateMap<CreateAmphoraDto, AmphoraModel>()
                .ConstructUsing(_ => new AmphoraModel(_.Name, _.Description, _.Price, null))
                .ForMember(o => o.Id, p => p.Ignore())
                .ForMember(o => o.Organisation, p => p.Ignore())
                .ForMember(o => o.OrganisationId, p => p.Ignore())
                .ForMember(o => o.TermsAndConditions, p => p.Ignore())
                .ForMember(o => o.Labels, p => p.Ignore())
                .ForMember(o => o.IsPublic, p => p.MapFrom(src => true))
                .ForMember(o => o.Signals, p => p.Ignore())
                .ForMember(o => o.Purchases, p => p.Ignore())
                .ForMember(o => o.ttl, p => p.Ignore())
                .ForMember(o => o.IsDeleted, p => p.Ignore())
                .ForMember(o => o.CreatedBy, p => p.Ignore())
                .ForMember(o => o.CreatedById, p => p.Ignore())
                .ForMember(o => o.CreatedDate, p => p.Ignore())
                .ForMember(o => o.LastModified, p => p.Ignore())
                .ForMember(o => o.GeoLocation, p => p.MapFrom(src =>
                    src.Lat.HasValue && src.Lon.HasValue ? new GeoLocation(src.Lon.Value, src.Lat.Value) : null));

            CreateMap<AmphoraExtendedDto, AmphoraModel>()
            .IncludeBase<EntityDto, Entity>()
            .ForMember(o => o.Id, p => p.Ignore())
            .ForMember(o => o.Organisation, p => p.Ignore())
            .ForMember(o => o.TermsAndConditions, p => p.Ignore())
            .ForMember(o => o.Labels, p => p.Ignore())
            .ForMember(o => o.IsPublic, p => p.MapFrom(src => true))
            .ForMember(o => o.Signals, p => p.Ignore())
            .ForMember(o => o.Purchases, p => p.Ignore())
            .ForMember(o => o.ttl, p => p.Ignore())
            .ForMember(o => o.CreatedBy, p => p.Ignore())
            .ForMember(o => o.CreatedById, p => p.Ignore())
            .ForMember(o => o.CreatedDate, p => p.Ignore())
            .ForMember(o => o.GeoLocation, p => p.MapFrom(src =>
                src.Lat.HasValue && src.Lon.HasValue ? new GeoLocation(src.Lon.Value, src.Lat.Value) : null));

            CreateMap<AmphoraModel, AmphoraExtendedDto>()
            .IncludeBase<AmphoraModel, AmphoraDto>()
            .ForMember(o => o.Lat, p => p.MapFrom(src => src.GeoLocation.Lat()))
            .ForMember(o => o.Lon, p => p.MapFrom(src => src.GeoLocation.Lon()));

            CreateMap<SignalModel, SignalDto>();
        }
    }
}
