using System;
using Amphora.Common.Models.Permissions;
using FluentAssertions;
using Xunit;

namespace Amphora.Tests.Unit.Casting
{
    public class AccessLevelTests : UnitTestBase
    {
        [Fact]
        public void IntegerAccessLevel_CanBeCastToAccessLevel()
        {
            for (int i = 0; i <= 256; i++)
            {
                AccessLevels level = (AccessLevels)Enum.ToObject(typeof(AccessLevels), i);
                level.Should().NotBeNull("because it should be cast to the enum");
                level.Should().BeEquivalentTo(i, "because it was cast from i");
                ((int)level).Should().Be(i);
            }
        }
    }
}