using System;

namespace Amphora.Api.Contracts
{
    public interface ICache
    {
        void Compact(double percentage = 50);
        T Set<T>(string key, T value, TimeSpan slidingExpiration);
        T Set<T>(string key, T value, DateTimeOffset absoluteExpiration);
        bool TryGetValue<T>(string key, out T value);
    }
}