using System;
using Amphora.Tests.Mocks;
using FluentAssertions;
using Xunit;

namespace Amphora.Tests.Unit.Services
{
    public class DateTimeProviderTests : UnitTestBase
    {
        // these are for the mock date time provider, because it can be trixxy
        [Fact]
        public void CanGetNow_EmptyCtor()
        {
            var sut = new MockDateTimeProvider();
            sut.Now.Should().BeCloseTo(DateTimeOffset.Now);
            sut.UtcNow.Should().BeCloseTo(DateTimeOffset.UtcNow);
        }

        [Fact]
        public void CanGetNow_Fixed()
        {
            var now = DateTimeOffset.Now;
            var sut = new MockDateTimeProvider(now);
            sut.Now.Should().BeCloseTo(now);
            sut.UtcNow.Should().BeCloseTo(now.UtcDateTime);
        }
    }
}