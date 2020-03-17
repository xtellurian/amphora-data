using Amphora.Common.Models.Dtos.Users;
using Amphora.Common.Models.Users;
using AutoMapper;

namespace Amphora.Api.Models.AutoMapper
{
    public class ApplicationUserDataModelProfile : Profile
    {
        public ApplicationUserDataModelProfile()
        {
            CreateMap<ApplicationUserDataModel, AmphoraUser>()
                .ForMember(p => p.Email, o => o.Ignore())
                .ForMember(p => p.FullName, o => o.Ignore());
        }
    }
}
