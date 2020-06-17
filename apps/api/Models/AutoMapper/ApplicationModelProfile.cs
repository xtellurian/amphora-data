using Amphora.Api.Models.Dtos.Applications;
using Amphora.Common.Models.Applications;
using AutoMapper;

namespace Amphora.Api.Models.AutoMapper
{
    public class ApplicationModelProfile : Profile
    {
        public ApplicationModelProfile()
        {
            CreateMap<CreateApplication, ApplicationModel>()
            .ForMember(_ => _.Id, opt => opt.Ignore())
            .ForMember(_ => _.OrganisationId, opt => opt.Ignore())
            .ForMember(_ => _.IsDeleted, opt => opt.Ignore())
            .ForMember(_ => _.CreatedDate, opt => opt.Ignore())
            .ForMember(_ => _.ttl, opt => opt.Ignore())
            .ForMember(_ => _.AllowedGrantTypes, opt => opt.Ignore())
            .ForMember(_ => _.RequireConsent, opt => opt.Ignore())
            .ForMember(_ => _.AllowOffline, opt => opt.Ignore())
            .ForMember(_ => _.LastModified, opt => opt.Ignore());

            CreateMap<CreateAppLocation, ApplicationLocationModel>()
            .ForMember(_ => _.Id, opt => opt.Ignore())
            .ForMember(_ => _.IsDeleted, opt => opt.Ignore())
            .ForMember(_ => _.CreatedDate, opt => opt.Ignore())
            .ForMember(_ => _.ttl, opt => opt.Ignore())
            .ForMember(_ => _.ApplicationId, opt => opt.Ignore())
            .ForMember(_ => _.LastModified, opt => opt.Ignore());

            CreateMap<ApplicationModel, Application>();
            CreateMap<ApplicationLocationModel, AppLocation>();
        }
    }
}
