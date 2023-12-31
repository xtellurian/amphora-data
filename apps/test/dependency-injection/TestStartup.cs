using System.Net.Http;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Options;
using Amphora.Infrastructure.Services.Azure;
using Amphora.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Xunit.Abstractions;
using Xunit.DependencyInjection;

[assembly: TestFramework("Amphora.Tests.Unit.TestStartup", "test")]

namespace Amphora.Tests.Unit
{
    public class TestStartup : DependencyInjectionTestFramework
    {
        public TestStartup(IMessageSink messageSink) : base(messageSink) { }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IAzureServiceTokenProvider, MockTokenProvider>();
            services.Configure<IOptionsMonitor<TsiOptions>>(o =>
            {
                o.CurrentValue.DataAccessFqdn = null;
            });
            services.AddTransient<ITsiService, TsiService>();
            var mockFactory = new Mock<IHttpClientFactory>();
            var clientHandlerStub = new DelegatingHandlerStub();
            var client = new HttpClient(clientHandlerStub);
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            services.AddSingleton<IHttpClientFactory>(mockFactory.Object);
        }
    }
}