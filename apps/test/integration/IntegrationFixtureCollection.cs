using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration
{
    [CollectionDefinition(nameof(IntegrationFixtureCollection))]
    public class IntegrationFixtureCollection : ICollectionFixture<WebApplicationFactory<Amphora.Api.Startup>>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}