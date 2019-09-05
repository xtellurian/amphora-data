using Amphora.Api;
using Amphora.Api.Models;
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
        public void ConvertApplicationUserToTestUser()
        {
            var appUser = new ApplicationUser();
            var result = mapper.Map<TestApplicationUser>(appUser);
            Assert.NotNull(result);
            result = mapper.Map<TestApplicationUser>(appUser as IApplicationUser);
            Assert.NotNull(result);
        }

        [Fact]
        public void AssertMappingConfigurationIsValid()
        {
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}