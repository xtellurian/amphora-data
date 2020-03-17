using System;
using Amphora.Api;
using Amphora.Common.Models.Dtos.Users;
using Amphora.Common.Models.Users;
using AutoMapper;
using Xunit;

namespace Amphora.Tests.Unit.Automapper
{
    public class AutomapperConfig
    {
        private IMapper mapper;
        private Random rnd = new Random();

        public AutomapperConfig()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(Startup));
            });
            this.mapper = config.CreateMapper();
        }

        [Fact]
        public void AssertMapperConfigurationIsValid()
        {
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [Fact]
        public void ConvertUserModelToDTO()
        {
            var appUser = new ApplicationUserDataModel();
            var result = mapper.Map<AmphoraUser>(appUser);
            Assert.NotNull(result);
        }
    }
}