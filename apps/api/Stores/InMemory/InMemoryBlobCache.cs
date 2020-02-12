using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;

namespace Amphora.Api.Stores
{
    public class InMemoryBlobCache : IBlobCache
    {
        private Dictionary<string, object> store = new Dictionary<string, object>();
        public Task SetAsync<T>(string key, T value) where T : class
        {
            store[key] = value;
            return Task.CompletedTask;
        }

        public Task<T> TryGetValue<T>(string key) where T : class
        {
            if (store.ContainsKey(key))
            {
                return Task.FromResult(store[key] as T);
            }
            else
            {
                return Task.FromResult(default(T));
            }
        }
    }
}