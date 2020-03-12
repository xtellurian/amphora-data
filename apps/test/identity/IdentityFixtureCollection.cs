using Amphora.Tests.Setup;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests
{
    [CollectionDefinition(nameof(IdentityFixtureCollection))]
    public class IdentityFixtureCollection : ICollectionFixture<WebApplicationFactory<Amphora.Identity.Startup>>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}