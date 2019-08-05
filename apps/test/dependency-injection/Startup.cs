using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using Xunit.DependencyInjection;

[assembly: TestFramework("Amphora.Tests.Unit.TestStartup", "test-unit")]

namespace Amphora.Tests.Unit
{
    public class TestStartup : DependencyInjectionTestFramework
    {
        public TestStartup(IMessageSink messageSink) : base(messageSink) { }

        protected override void ConfigureServices(IServiceCollection services)
        {
            // services.AddTransient<IDependency, DependencyClass>();
        }
    }
}