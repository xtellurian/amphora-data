using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;

namespace Amphora.Api.Stores
{
    public class InMemoryDataStore<T, TData> : IDataStore<T, TData> where T : class, IOrgScoped
    {
        private Dictionary<string, TData> store = new Dictionary<string, TData>();
        public Task<TData> GetDataAsync(T entity)
        {
            return Task<TData>.Factory.StartNew(() =>
            {

                if (entity?.Id == null) throw new ArgumentException();
                if (store.ContainsKey(entity.Id))
                {
                    return store[entity.Id];
                }
                else
                {
                    return default(TData);
                }
            });
        }

        public Task<TData> SetDataAsync(T entity, TData data)
        {
            return Task<TData>.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(entity.Id))
                {
                    entity.Id = Guid.NewGuid().ToString();
                }
                store[entity.Id] = data;
                return data;
            });
        }
    }
}