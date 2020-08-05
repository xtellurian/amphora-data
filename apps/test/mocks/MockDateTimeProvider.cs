using System;
using Amphora.Common.Contracts;

namespace Amphora.Tests.Mocks
{
    public class MockDateTimeProvider : IDateTimeProvider
    {
        private readonly DateTimeOffset? minValue;
        private DateTimeOffset? fixedNow;

        public MockDateTimeProvider(DateTimeOffset? fixedNow = null)
        {
            this.fixedNow = fixedNow;
        }

        public MockDateTimeProvider(DateTimeOffset fixedNow, DateTimeOffset? minValue) : this(fixedNow)
        {
            this.minValue = minValue;
        }

        public void Reset()
        {
            fixedNow = null;
        }

        public void SetFixed(DateTimeOffset fixedNow)
        {
            this.fixedNow = fixedNow;
        }

        public DateTimeOffset Now => fixedNow ?? DateTimeOffset.Now;

        public DateTimeOffset UtcNow => Now.UtcDateTime;

        public DateTimeOffset MinValue => minValue ?? DateTimeOffset.MinValue;
    }
}