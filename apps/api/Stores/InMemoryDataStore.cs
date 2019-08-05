using System;
using System.Collections.Generic;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;

namespace Amphora.Api.Stores
{
    public class InMemoryDataStore<T, TData> : IDataStore<T,TData> where T: class, IDataEntity
    {
        private Dictionary<T, TData> store = new Dictionary<T, TData>();
        public TData GetData(T entity)
        {
            if(store.ContainsKey(entity))
            {
                return store[entity];
            }
            else
            {
                return default;
            }
        }

        public TData SetData(T entity, TData data)
        {
            if(string.IsNullOrEmpty(entity.Id))
            {
                entity.Id = Guid.NewGuid().ToString();
            }
            store[entity] = data;
            return data;
        }
    }
}