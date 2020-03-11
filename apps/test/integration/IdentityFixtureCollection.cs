using Amphora.Tests.Setup;
using Xunit;

namespace Amphora.Tests
{
    [CollectionDefinition(nameof(IdentityFixtureCollection))]
    public class IdentityFixtureCollection : ICollectionFixture<IdentityWebApplicationFactory>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}