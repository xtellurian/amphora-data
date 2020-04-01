using System;

namespace Amphora.Common.Models.Organisations
{
    public class DataCache
    {
        public CachedValue<long>? TotalAmphoraeFileSize { get; set; }

        public class CachedValue<T> where T : struct
        {
            public CachedValue(T? value, DateTimeOffset? lastUpdated = null)
            {
                LastUpdated = lastUpdated ?? System.DateTime.UtcNow;
                Value = value;
            }

            public DateTimeOffset LastUpdatedNotNull => LastUpdated ?? DateTimeOffset.MinValue;
            public DateTimeOffset? LastUpdated { get; set; }
            public T? Value { get; set; }
        }
    }
}