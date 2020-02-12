using System;
using Amphora.Api.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace Amphora.Api.Services.Wrappers
{
    public class InMemoryCache : ICache
    {
        public MemoryCache Cache { get; set; }

        public InMemoryCache()
        {
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 512
            });
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            return Cache.TryGetValue(key, out value);
        }

        public T Set<T>(string key, T value, TimeSpan slidingExpiration)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
               .SetSize(1)
               .SetSlidingExpiration(TimeSpan.FromSeconds(3));

            return Cache.Set(key, value, cacheEntryOptions);
        }

        public T Set<T>(string key, T value, DateTimeOffset absoluteExpiration)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
               .SetSize(1)
               .SetAbsoluteExpiration(absoluteExpiration);

            return Cache.Set(key, value, cacheEntryOptions);
        }

        public void Compact(double percentage = 50)
        {
            Cache.Compact(percentage);
        }
    }
}