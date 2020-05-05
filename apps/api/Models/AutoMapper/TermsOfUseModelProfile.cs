using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using AutoMapper;

namespace Amphora.Api.Models.AutoMapper
{
    public class TermsOfUseModelProfile : Profile
    {
        public TermsOfUseModelProfile()
        {
            CreateMap<CreateTermsOfUse, TermsOfUseModel>()
            .ForMember(m => m.OrganisationId, o => o.Ignore())
            .ForMember(m => m.Organisation, o => o.Ignore())
            .ForMember(m => m.Id, o => o.Ignore())
            .ForMember(m => m.IsDeleted, o => o.Ignore())
            .ForMember(m => m.CreatedDate, o => o.Ignore())
            .ForMember(m => m.LastModified, o => o.Ignore())
            .ForMember(m => m.ttl, o => o.Ignore())
            .ReverseMap();

            CreateMap<TermsOfUseModel, TermsOfUse>();
        }
    }
}
