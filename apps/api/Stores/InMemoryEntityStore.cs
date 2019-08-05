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
        protected List<T> store = new List<T>();

        public T Get(string id)
        {
            return this.store.FirstOrDefault(o => string.Equals(o.Id, id));
        }

        public IReadOnlyCollection<T> List()
        {
            return new ReadOnlyCollection<T> (this.store);
        }

        public IEnumerable<string> ListIds()
        {
            return this.store.Select(o => o.Id);
        }

        public T Set(T model)
        {
            if(string.IsNullOrEmpty(model.Id)) 
            {
                model.Id = Guid.NewGuid().ToString();
            }
            if(this.store.Any( o => o.Id == model.Id))
            {
                this.store.RemoveAll(o => o.Id == model.Id);
            }

            this.store.Add(model);
            return model;
        }
    }
}