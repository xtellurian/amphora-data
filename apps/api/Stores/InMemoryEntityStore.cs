using System;
using System.Collections.Generic;
using api.Contracts;
using common.Contracts;

namespace api.Stores
{
    public class InMemoryEntityStore<T>: IAmphoraEntityStore<T> where T: class, IAmphoraEntity
    {
        private Dictionary<string, T> store = new Dictionary<string, T>();

        public T Get(string id)
        {
            if (this.store.ContainsKey(id)){
                return this.store[id];
            }
            else 
            {
                return null;
            }
        }

        public IEnumerable<string> ListIds()
        {
            return this.store.Keys;
        }

        public T Set(T model)
        {
            if(string.IsNullOrEmpty(model.Id)) 
            {
                model.Id = Guid.NewGuid().ToString();
            }

            this.store[model.Id] = model;
            return model;
        }
    }
}