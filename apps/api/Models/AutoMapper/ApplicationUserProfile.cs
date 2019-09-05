using AutoMapper;

namespace Amphora.Api.Models.AutoMapper
{
    public class ApplicationUserProfile: Profile
    {
        public ApplicationUserProfile()
        {
            CreateMap<ApplicationUser, TestApplicationUser>()
            .ForMember(p => p.ConcurrencyStamp, o => o.Ignore())
            .ForMember(p => p.LockoutEnd, o => o.Ignore());
        }
    }
}
