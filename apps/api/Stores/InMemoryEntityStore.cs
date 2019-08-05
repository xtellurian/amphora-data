using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;

namespace Amphora.Api.Stores
{
    public class InMemoryEntityStore<T>: IEntityStore<T> where T: class, IEntity
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

        public IReadOnlyCollection<T> List()
        {
            return new ReadOnlyCollection<T> (this.store.Values.ToList());
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