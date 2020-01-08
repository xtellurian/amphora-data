using System;
using Amphora.Common.Contracts;

namespace Amphora.Tests.Mocks
{
    public class MockDateTimeProvider : IDateTimeProvider
    {
        public MockDateTimeProvider()
        {
            Now = DateTime.Now;
        }

        public MockDateTimeProvider(DateTimeOffset now)
        {
            Now = now;
        }

        public DateTimeOffset Now { get; set; }

        public DateTimeOffset UtcNow => Now.UtcDateTime;
    }
}