using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;

namespace Amphora.Api.Stores
{
    public class InMemoryDataStore<T, TData> : IDataStore<T, TData> where T : class, IEntity
    {
        private Dictionary<string, Dictionary<string,TData>> store = new Dictionary<string, Dictionary<string, TData>>();
        public Task<TData> GetDataAsync(T entity, string name)
        {
            return Task<TData>.Factory.StartNew(() =>
            {

                if (entity?.Id == null) throw new ArgumentException();
                if (store.ContainsKey(entity.Id))
                {
                    var dataStore = store[entity.Id];
                    if(dataStore.ContainsKey(name))
                    {
                        return dataStore[name];   
                    }
                    else
                    {
                        return default(TData);
                    }
                }
                else
                {
                    return default(TData);
                }
            });
        }

        public Task<TData> SetDataAsync(T entity, TData data, string name)
        {
            return Task<TData>.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(entity.Id))
                {
                    entity.Id = Guid.NewGuid().ToString();
                }
                if(! store.ContainsKey(entity.Id))
                {
                    store[entity.Id] = new Dictionary<string, TData>();
                }
                var dataStore = store[entity.Id];
                dataStore[name] = data;
                return data;
            });
        }

        public Task<IEnumerable<string>> ListNamesAsync(T entity)
        {
            return Task<IEnumerable<string>>.Factory.StartNew( () => {
                if(store.ContainsKey(entity.Id))
                {
                    var entityData = store[entity.Id];
                    return entityData.Keys;
                }
                else
                {
                    return new List<string>();
                }
            });
        }
    }
}