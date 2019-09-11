using Amphora.Common.Models.Organisations;
using AutoMapper;

namespace Amphora.Api.Models.AutoMapper
{
    public class OrganisationModelProfile : Profile
    {
        public OrganisationModelProfile()
        {
            CreateMap<OrganisationModel, OrganisationExtendedModel>()
            .IncludeAllDerived()
            .ForMember(o => o.Invitations, p => p.Ignore());
        }
    }
}
