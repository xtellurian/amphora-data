using Amphora.Tests.Setup;
using Xunit;

namespace Amphora.Tests
{
    [CollectionDefinition(nameof(InMemoryIdentityFixtureCollection))]
    public class InMemoryIdentityFixtureCollection : ICollectionFixture<InMemoryIdentityWebApplicationFactory>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}