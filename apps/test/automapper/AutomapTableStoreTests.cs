using System;
using Amphora.Api;
using Amphora.Api.Models;
using Amphora.Common.Models;
using AutoMapper;
using NGeoHash;
using Xunit;

namespace Amphora.Tests.Unit.Automapper
{
    public class AutomapTableStoreTests
    {
        private IMapper mapper;
        private Random rnd = new Random();

        public AutomapTableStoreTests()
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
        public void MapAmphoraToTableStoreEntity()
        {
            var amphora = new Amphora.Common.Models.Amphora()
            {
                // set random things
            };
            var tableEntity = mapper.Map<AmphoraTableEntity>(amphora);
            Assert.NotNull(tableEntity);
        }

        [Fact]
        public void MapTableStoreEntityToAmphora()
        {
            var tableEntity = new AmphoraTableEntity()
            {
                // set random things
            };
            var amphora = mapper.Map<Amphora.Common.Models.Amphora>(tableEntity);
            Assert.NotNull(amphora);
        }

        [Fact]
        public void TestPositionMapping()
        {
            var geoHash = GeoHash.Encode(rnd.Next(0,180), rnd.Next(0,180));
            var amphora = new Amphora.Common.Models.Amphora()
            {
                GeoHash = geoHash
            };
            var tableEntity = mapper.Map<AmphoraTableEntity>(amphora);
            Assert.NotNull(tableEntity);
            var entity = mapper.Map<Amphora.Common.Models.Amphora>(tableEntity);
            Assert.Equal(geoHash, entity.GeoHash);
        }
    }
}