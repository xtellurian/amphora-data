using Amphora.Api.Models.Dtos.Accounts.Memberships;
using AutoMapper;

namespace Amphora.Api.Models.AutoMapper
{
    public class MembershipProfile : Profile
    {
        public MembershipProfile()
        {
            CreateMap<Common.Models.Organisations.Membership, Membership>()
            .ForMember(_ => _.Username, o => o.MapFrom(src => src.User.UserName));
        }
    }
}