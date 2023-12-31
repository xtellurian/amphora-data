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
            CreateMap<Organisation, OrganisationModel>()
            .IncludeBase<Amphora.Api.Models.Dtos.Entity, EntityBase>()
            .ForMember(p => p.Account, o => o.Ignore())
            .ForMember(p => p.Cache, o => o.Ignore())
            .ForMember(p => p.Configuration, o => o.Ignore())
            .ForMember(p => p.GlobalInvitations, o => o.Ignore())
            .ForMember(p => p.Invoices, o => o.Ignore())
            .ForMember(p => p.IsAutogenerated, o => o.Ignore())
            .ForMember(p => p.Memberships, o => o.Ignore())
            .ForMember(p => p.TermsOfUses, o => o.Ignore())
            .ForMember(p => p.TermsOfUsesAccepted, o => o.Ignore())
            .ForMember(p => p.Purchases, o => o.Ignore())
            .ForMember(p => p.CreatedById, o => o.Ignore())
            .ForMember(p => p.CreatedBy, o => o.Ignore())
            .ForMember(p => p.ttl, o => o.MapFrom(src => -1))
            .ForMember(p => p.CreatedDate, o => o.Ignore())
            .ReverseMap();

            CreateMap<Common.Models.Organisations.Accounts.Account, Dtos.Accounts.AccountInformation>();
        }
    }
}
