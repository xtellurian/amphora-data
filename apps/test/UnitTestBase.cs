using Amphora.Api;
using Amphora.Api.DbContexts;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Amphora.Tests.Unit
{
    public abstract class UnitTestBase
    {
        protected UnitTestBase()
        {
            var config = new MapperConfiguration(cfg =>
           {
               cfg.AddMaps(typeof(Startup));
           });
            this.Mapper = config.CreateMapper();
        }

        protected AmphoraContext GetContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<AmphoraContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            // Run the test against one instance of the context
            return new AmphoraContext(options);
        }

        protected IMapper Mapper { get; }
    }
}