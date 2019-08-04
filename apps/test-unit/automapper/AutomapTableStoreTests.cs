using System.Linq;
using System.Reflection;
using Amphora.Api;
using Amphora.Api.Models;
using Amphora.Common.Models;
using AutoMapper;
using Xunit;

namespace Amphora.Tests.Unit.Automapper
{
    public class AutomapTableStoreTests
    {
        private IMapper mapper;

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
            var amphora = new AmphoraModel()
            {
                // set random things
            };
            var tableEntity = mapper.Map<AmphoraModelTableEntity>(amphora);
            Assert.NotNull(tableEntity);
        }

        [Fact]
        public void MapTableStoreEntityToAmphora()
        {
            var tableEntity = new AmphoraModelTableEntity()
            {
                // set random things
            };
            var amphora = mapper.Map<AmphoraModel>(tableEntity);
            Assert.NotNull(amphora);
        }

        [Fact]
        public void MapSchemaToTableStoreEntity()
        {
            var schema = new AmphoraSchema()
            {
                // set random things
            };
            var tableEntity = mapper.Map<AmphoraSchemaTableEntity>(schema);
            Assert.NotNull(tableEntity);
        }

        [Fact]
        public void MapTableStoreEntityToSchema()
        {
            var tableEntity = new AmphoraSchemaTableEntity()
            {
                // set random things
            };
            var schema = mapper.Map<AmphoraSchema>(tableEntity);
            Assert.NotNull(schema);
        }
    }
}