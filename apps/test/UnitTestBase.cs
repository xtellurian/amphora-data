using Amphora.Api;
using AutoMapper;

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

        protected IMapper Mapper { get; }
    }
}