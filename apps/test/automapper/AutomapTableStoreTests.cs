using System;
using Amphora.Api;
using Amphora.Api.Models;
using Amphora.Common.Models;
using AutoMapper;
using NGeoHash;
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
    }
}