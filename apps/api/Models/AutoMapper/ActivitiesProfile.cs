using Amphora.Api.Models.Dtos.Activities;
using Amphora.Common.Models.Activities;
using AutoMapper;

namespace Amphora.Api.Models.AutoMapper
{
    public class ActivitiesProfile : Profile
    {
        public ActivitiesProfile()
        {
            CreateMap<CreateActivity, ActivityModel>()
            .ForMember(_ => _.Id, opt => opt.Ignore())
            .ForMember(_ => _.Runs, opt => opt.Ignore())
            .ForMember(_ => _.OrganisationId, opt => opt.Ignore())
            .ForMember(_ => _.Organisation, opt => opt.Ignore())
            .ForMember(_ => _.IsDeleted, opt => opt.Ignore())
            .ForMember(_ => _.CreatedDate, opt => opt.Ignore())
            .ForMember(_ => _.ttl, opt => opt.Ignore())
            .ForMember(_ => _.LastModified, opt => opt.Ignore());

            CreateMap<ActivityModel, Activity>();

            CreateMap<Run, ActivityRunModel>()
            .ForMember(_ => _.StartedByUserId, opt => opt.Ignore())
            .ForMember(_ => _.StartedByUser, opt => opt.Ignore())
            .ForMember(_ => _.Activity, opt => opt.Ignore())
            .ForMember(_ => _.ActivityId, opt => opt.Ignore())
            .ForMember(_ => _.ttl, opt => opt.Ignore())
            .ForMember(_ => _.IsDeleted, opt => opt.Ignore())
            .ForMember(_ => _.CreatedDate, opt => opt.Ignore())
            .ForMember(_ => _.LastModified, opt => opt.Ignore());

            CreateMap<ActivityRunModel, Run>()
            .ForMember(_ => _.StartedBy, opt => opt.MapFrom(src => src.StartedByUser.UserName));

            CreateMap<AmphoraReference, ActivityAmphoraReference>()
            .ForMember(_ => _.Id, opt => opt.Ignore())
            .ForMember(_ => _.Amphora, opt => opt.Ignore())
            .ReverseMap();
        }
    }
}


// AmphoraReferences