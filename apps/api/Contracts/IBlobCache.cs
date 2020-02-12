using System;
using System.Threading.Tasks;

namespace Amphora.Api.Contracts
{
    public interface IBlobCache
    {
        Task SetAsync<T>(string key, T value) where T : class;
        Task<T> TryGetValue<T>(string key) where T : class;
    }
}