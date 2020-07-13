using System;
using Amphora.Common.Contracts;

namespace Amphora.Tests.Mocks
{
    public class MockDateTimeProvider : IDateTimeProvider
    {
        private readonly DateTimeOffset? minValue;

        public MockDateTimeProvider()
        {
            Now = DateTime.Now;
        }

        public MockDateTimeProvider(DateTimeOffset now, DateTimeOffset? minValue = null)
        {
            Now = now;
            this.minValue = minValue;
        }

        public DateTimeOffset Now { get; set; }

        public DateTimeOffset UtcNow => Now.UtcDateTime;

        public DateTimeOffset MinValue => minValue ?? DateTimeOffset.MinValue;
    }
}