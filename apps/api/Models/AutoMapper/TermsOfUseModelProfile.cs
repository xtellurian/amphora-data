using Amphora.Api.Models.Dtos.Terms;
using Amphora.Common.Models.Amphorae;
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
            .ForMember(m => m.AppliedToAmphoras, o => o.Ignore())
            .ForMember(m => m.Id, o => o.Ignore())
            .ForMember(m => m.IsDeleted, o => o.Ignore())
            .ForMember(m => m.CreatedDate, o => o.Ignore())
            .ForMember(m => m.LastModified, o => o.Ignore())
            .ForMember(m => m.ttl, o => o.Ignore());

            CreateMap<TermsOfUseModel, TermsOfUse>();
        }
    }
}
