using System;

namespace Amphora.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTimeOffset StartOfMonth(this DateTimeOffset dt)
        {
            return new System.DateTime(dt.Year, dt.Month, 1, 0, 0, 0, 0);
        }

        public static DateTimeOffset StartOfMonth(this DateTime dt)
        {
            return new System.DateTime(dt.Year, dt.Month, 1, 0, 0, 0, 0);
        }

        public static DateTimeOffset EndOfMonth(this DateTimeOffset dt)
        {
            var maxDays = DateTime.DaysInMonth(dt.Year, dt.Month);
            return new System.DateTime(dt.Year, dt.Month, maxDays, 23, 59, 50, 999);
        }

        public static DateTimeOffset EndOfMonth(this DateTime dt)
        {
            var maxDays = DateTime.DaysInMonth(dt.Year, dt.Month);
            return new System.DateTime(dt.Year, dt.Month, maxDays, 23, 59, 50, 999);
        }
    }
}