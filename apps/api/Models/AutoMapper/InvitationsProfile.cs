using Amphora.Api.Models.Dtos.Platform;
using Amphora.Common.Models.Platform;
using AutoMapper;

namespace Amphora.Api.Models.AutoMapper
{
    public class InvitationsProfile : Profile
    {
        public InvitationsProfile()
        {
            CreateMap<InvitationDto, InvitationModel>()
                .ForMember(p => p.TargetOrganisation, o => o.Ignore())
                .ForMember(p => p.TargetDomain, o => o.Ignore())
                .ForMember(p => p.IsClaimed, o => o.Ignore())
                .ForMember(p => p.Id, o => o.Ignore())
                .ForMember(p => p.IsGlobalAdmin, o => o.Ignore())
                .ForMember(p => p.ttl, o => o.Ignore())
                .ForMember(p => p.IsDeleted, o => o.Ignore())
                .ForMember(p => p.CreatedDate, o => o.Ignore());
        }
    }
}