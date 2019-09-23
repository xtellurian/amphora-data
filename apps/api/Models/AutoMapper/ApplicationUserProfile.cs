using Amphora.Api.Models.Users;
using Amphora.Common.Contracts;
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

            CreateMap<IApplicationUser, ApplicationUserDto>();
            CreateMap<IApplicationUser, TestApplicationUser>()
            .ForMember(p => p.NormalizedUserName, o => o.Ignore())
            .ForMember(p => p.NormalizedEmail, o => o.Ignore())
            .ForMember(p => p.EmailConfirmed, o => o.Ignore())
            .ForMember(p => p.PasswordHash, o => o.Ignore())
            .ForMember(p => p.SecurityStamp, o => o.Ignore())
            .ForMember(p => p.ConcurrencyStamp, o => o.Ignore())
            .ForMember(p => p.PhoneNumber, o => o.Ignore())
            .ForMember(p => p.PhoneNumberConfirmed, o => o.Ignore())
            .ForMember(p => p.TwoFactorEnabled, o => o.Ignore())
            .ForMember(p => p.LockoutEnd, o => o.Ignore())
            .ForMember(p => p.LockoutEnabled, o => o.Ignore())
            .ForMember(p => p.AccessFailedCount, o => o.Ignore());

            CreateMap<IApplicationUser, ApplicationUser>()
            .ForMember(p => p.PartitionKey, o => o.Ignore())
            .ForMember(p => p.NormalizedUserName, o => o.Ignore())
            .ForMember(p => p.SecurityStamp, o => o.Ignore())
            .ForMember(p => p.NormalizedEmail, o => o.Ignore())
            .ForMember(p => p.EmailConfirmed, o => o.Ignore())
            .ForMember(p => p.PhoneNumber, o => o.Ignore())
            .ForMember(p => p.PhoneNumberConfirmed, o => o.Ignore())
            .ForMember(p => p.TwoFactorEnabled, o => o.Ignore())
            .ForMember(p => p.LockoutEndDateUtc, o => o.Ignore())
            .ForMember(p => p.LockoutEnabled, o => o.Ignore())
            .ForMember(p => p.AccessFailedCount, o => o.Ignore())
            .ForMember(p => p.Roles, o => o.Ignore())
            .ForMember(p => p.PasswordHash, o => o.Ignore())
            .ForMember(p => p.Logins, o => o.Ignore())
            .ForMember(p => p.Tokens, o => o.Ignore())
            .ForMember(p => p.Claims, o => o.Ignore())
            .ForMember(p => p.ResourceId, o => o.Ignore())
            .ForMember(p => p.SelfLink, o => o.Ignore())
            .ForMember(p => p.AltLink, o => o.Ignore())
            .ForMember(p => p.Timestamp, o => o.Ignore())
            .ForMember(p => p.ETag, o => o.Ignore());
        }
    }
}
