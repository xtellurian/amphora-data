using System;
using System.Collections.Generic;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;

namespace Amphora.Api.Stores
{
    public class InMemoryDataStore<T, TData> : IDataStore<T,TData> where T: class, IOrgScoped
    {
        private Dictionary<string, TData> store = new Dictionary<string, TData>();
        public TData GetData(T entity)
        {
            if(entity?.Id == null) throw new ArgumentException();
            if(store.ContainsKey(entity.Id))
            {
                return store[entity.Id];
            }
            else
            {
                return default(TData);
            }
        }

        public TData SetData(T entity, TData data)
        {
            if(string.IsNullOrEmpty(entity.Id))
            {
                entity.Id = Guid.NewGuid().ToString();
            }
            store[entity.Id] = data;
            return data;
        }
    }
}