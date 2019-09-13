using Amphora.Common.Models.Organisations;
using AutoMapper;

namespace Amphora.Api.Models.AutoMapper
{
    public class OrganisationModelProfile : Profile
    {
        public OrganisationModelProfile()
        {
            // this is to extend a base class into its extended version. 
            // Generally only used in the InMemory stores.
            CreateMap<OrganisationModel, OrganisationExtendedModel>()
            .IncludeAllDerived()
            .ForMember(o => o.Invitations, p => p.Ignore())
            .ForMember(o => o.Memberships, p => p.Ignore())
            .ForMember(o => o.WebsiteUrl, p => p.Ignore())
            .ForMember(o => o.About, p => p.Ignore())
            .ForMember(o => o.Address, p => p.Ignore());
        }
    }
}
