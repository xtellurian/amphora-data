using System;
using Amphora.Common.Contracts;

namespace Amphora.Common.Services.Timing
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset Now => DateTimeOffset.Now;
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
        public DateTimeOffset MinValue => DateTimeOffset.MinValue;
    }
}