using Amphora.Api.Models.Dtos;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Common.Models;
using Amphora.Common.Models.Organisations;
using AutoMapper;

namespace Amphora.Api.Models.AutoMapper
{
    public class OrganisationModelProfile : Profile
    {
        public OrganisationModelProfile()
        {
            CreateMap<OrganisationDto, OrganisationModel>()
            .IncludeBase<EntityDto, Entity>()
            .ForMember(p => p.Account, o => o.Ignore())
            .ForMember(p => p.GlobalInvitations, o => o.Ignore())
            .ForMember(p => p.Memberships, o => o.Ignore())
            .ForMember(p => p.TermsAndConditions, o => o.Ignore())
            .ForMember(p => p.TermsAndConditionsAccepted, o => o.Ignore())
            .ForMember(p => p.CreatedById, o => o.Ignore())
            .ForMember(p => p.CreatedBy, o => o.Ignore())
            .ForMember(p => p.ttl, o => o.MapFrom(src => -1))
            .ForMember(p => p.CreatedDate, o => o.Ignore())
            .ReverseMap();

            CreateMap<TermsAndConditionsDto, TermsAndConditionsModel>()
            .ForMember(m => m.OrganisationId, o => o.Ignore())
            .ForMember(m => m.Organisation, o => o.Ignore())
            .ReverseMap();

            CreateMap<Common.Models.Organisations.Accounts.Account, Dtos.Organisations.Account>();

        }
    }
}
