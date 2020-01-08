using System;
using Amphora.Common.Contracts;

namespace Amphora.Common.Services.Timing
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset Now => DateTime.Now;

        public DateTimeOffset UtcNow => DateTime.UtcNow;
    }
}