using System;
using Amphora.Common.Contracts;

namespace Amphora.Tests.Mocks
{
    public class MockDateTimeProvider : IDateTimeProvider
    {
        private readonly DateTimeOffset? minValue;

        public MockDateTimeProvider(DateTimeOffset? now = null)
        {
            GetNow = () => now ?? DateTimeOffset.Now;
        }

        public MockDateTimeProvider(DateTimeOffset now, DateTimeOffset? minValue) : this(now)
        {
            this.minValue = minValue;
        }

        public void Reset()
        {
            GetNow = () => DateTimeOffset.Now;
        }

        public Func<DateTimeOffset> GetNow { get; set; }
        public DateTimeOffset Now => GetNow();

        public DateTimeOffset UtcNow => Now.UtcDateTime;

        public DateTimeOffset MinValue => minValue ?? DateTimeOffset.MinValue;
    }
}