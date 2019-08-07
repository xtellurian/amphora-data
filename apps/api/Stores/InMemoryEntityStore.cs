using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;

namespace Amphora.Api.Stores
{
    public class InMemoryEntityStore<T> : IEntityStore<T> where T : class, IEntity
    {
        protected List<T> store = new List<T>();

        public T Get(string id)
        {
            return this.store.FirstOrDefault(o => string.Equals(o.Id, id));
        }

        public Task<T> GetAsync(string id)
        {
            return Task<T>.Factory.StartNew(() =>
            {
                return store.FirstOrDefault(e => string.Equals(e.Id, id));
            });
        }

        public Task<IList<T>> ListAsync()
        {
            return Task<IList<T>>.Factory.StartNew(() =>
            {
                return new ReadOnlyCollection<T>(this.store);
            });
        }

        public Task<T> SetAsync(T entity)
        {
            return Task<T>.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(entity.Id))
                {
                    entity.Id = Guid.NewGuid().ToString();
                }
                if (this.store.Any(o => o.Id == entity.Id))
                {
                    this.store.RemoveAll(o => o.Id == entity.Id);
                }

                this.store.Add(entity);
                return entity;
            });
        }
    }
}