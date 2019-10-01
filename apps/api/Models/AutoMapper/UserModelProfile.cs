using Amphora.Common.Models.Users;
using AutoMapper;

namespace Amphora.Api.Models.AutoMapper
{
    public class UserModelProfile : Profile
    {
        public UserModelProfile()
        {

            CreateMap<ApplicationUser, UserDto>();

        }
    }
}
