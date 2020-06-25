using System;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Extensions;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using AutoMapper;

namespace Amphora.Api.Models.AutoMapper
{
    public class AmphoraModelProfile : Profile
    {
        public AmphoraModelProfile()
        {
            CreateMap<AmphoraModel, BasicAmphora>()
                .ForMember(o => o.Labels, p => p.MapFrom(src => src.Labels.ToLabelString()))
                .IncludeBase<EntityBase, Amphora.Api.Models.Dtos.Entity>();

            CreateMap<AmphoraModel, EditAmphora>()
                .ForMember(o => o.Labels, p => p.MapFrom(src => src.Labels.ToLabelString()))
                .ForMember(o => o.Lat, p => p.MapFrom(src => src.GeoLocation.Lat()))
                .ForMember(o => o.Lon, p => p.MapFrom(src => src.GeoLocation.Lon()));

            CreateMap<CreateAmphora, AmphoraModel>()
                .ConstructUsing(_ => new AmphoraModel(_.Name, _.Description, _.Price, null))
                .ForMember(o => o.Id, p => p.Ignore())
                .ForMember(o => o.Organisation, p => p.Ignore())
                .ForMember(o => o.Labels, p => p.MapFrom(src => src.GetLabels()))
                .ForMember(o => o.OrganisationId, p => p.Ignore())
                .ForMember(o => o.TermsOfUse, p => p.Ignore())
                .ForMember(o => o.Quality, p => p.Ignore())
                .ForMember(o => o.AccessControl, p => p.Ignore())
                .ForMember(o => o.V2Signals, p => p.Ignore())
                .ForMember(o => o.IsPublic, p => p.MapFrom(src => true))
                .ForMember(o => o.Purchases, p => p.Ignore())
                .ForMember(o => o.PurchaseCount, p => p.Ignore())
                .ForMember(o => o.ttl, p => p.Ignore())
                .ForMember(o => o.IsDeleted, p => p.Ignore())
                .ForMember(o => o.CreatedBy, p => p.Ignore())
                .ForMember(o => o.CreatedById, p => p.Ignore())
                .ForMember(o => o.CreatedDate, p => p.Ignore())
                .ForMember(o => o.LastModified, p => p.Ignore())
                .ForMember(o => o.GeoLocation, p => p.MapFrom(src =>
                    src.Lat.HasValue && src.Lon.HasValue ? new GeoLocation(src.Lon.Value, src.Lat.Value) : null));

            CreateMap<AmphoraModel, DetailedAmphora>()
                .IncludeBase<AmphoraModel, BasicAmphora>()
                .ForMember(o => o.Lat, p => p.MapFrom(src => src.GeoLocation.Lat()))
                .ForMember(o => o.Lon, p => p.MapFrom(src => src.GeoLocation.Lon()))
                .ForMember(o => o.SignalCount, p => p.MapFrom(src => src.V2Signals.Count));

            CreateMap<DataQuality, Quality>();
            CreateMap<Quality, DataQuality>();
        }
    }
}
