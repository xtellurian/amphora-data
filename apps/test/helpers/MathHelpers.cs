using System;
using Xunit;

namespace Amphora.Tests.Helpers
{
    public static class MathHelpers
    {
        public static void AssertCloseEnough(double? expected, double? actual, double delta = 0.01)
        {
            Assert.True(expected.HasValue);
            Assert.True(actual.HasValue);
            Assert.True(Math.Abs(expected.Value - actual.Value) < delta);
        }
    }
}