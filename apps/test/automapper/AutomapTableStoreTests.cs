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
        public void MapSchemaToTableStoreEntity()
        {
            var schema = new Schema()
            {
                // set random things
            };
            var tableEntity = mapper.Map<SchemaTableEntity>(schema);
            Assert.NotNull(tableEntity);
        }

        [Fact]
        public void MapTableStoreEntityToSchema()
        {
            var tableEntity = new SchemaTableEntity()
            {
                // set random things
            };
            var schema = mapper.Map<Schema>(tableEntity);
            Assert.NotNull(schema);
        }
    }
}