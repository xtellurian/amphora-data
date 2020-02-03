using Amphora.Api;
using Amphora.Common.Models.Dtos.Users;
using Amphora.Common.Models.Users;
using AutoMapper;
using Xunit;

namespace Amphora.Tests.Unit
{
    public class AutomapApplicationUserTests
    {
        private readonly IMapper mapper;

        public AutomapApplicationUserTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(Startup));
            });
            this.mapper = config.CreateMapper();
        }

        [Fact]
        public void ConvertUserModelToDTO()
        {
            var appUser = new ApplicationUser();
            var result = mapper.Map<AmphoraUser>(appUser);
            Assert.NotNull(result);
        }

        [Fact]
        public void AssertMappingConfigurationIsValid()
        {
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}