using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Contracts;

namespace Amphora.Api.Stores
{
    public class InMemoryEntityStore<T> : IEntityStore<T> where T : class, IEntity
    {
        protected List<T> store = new List<T>();

        public Task<IList<T>> ListAsync()
        {
            return Task<IList<T>>.Factory.StartNew(() =>
            {
                return new ReadOnlyCollection<T>(this.store);
            });
        }

        public Task<T> CreateAsync(T entity)
        {
            return Task<T>.Factory.StartNew(() =>
            {
                entity.SetIds();
                if (this.store.Any(o => o.Id == entity.Id))
                {
                    this.store.RemoveAll(o => o.Id == entity.Id);
                }

                this.store.Add(entity);
                return entity;
            });
        }

        public Task<T> ReadAsync(string id)
        {
            return Task<T>.Factory.StartNew(() =>
            {
                if (id == null) return default(T);
                var qualifiedId = id.AsQualifiedId(typeof(T));
                return store.FirstOrDefault(e => string.Equals(e.Id, qualifiedId));
            });
        }

        public Task<T> UpdateAsync(T entity)
        {
            return Task<T>.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(entity.Id))
                {
                    throw new ArgumentException("id null of entity update.");
                }
                if (this.store.Any(o => o.Id == entity.Id))
                {
                    this.store.RemoveAll(o => o.Id == entity.Id);
                }

                this.store.Add(entity);
                return entity;
            });
        }

        public Task DeleteAsync(T entity)
        {
            return Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(entity.Id))
                {
                    throw new ArgumentException("id null of entity update.");
                }
                this.store.RemoveAll(o => o.Id == entity.Id);
            });
        }

        public Task<IList<T>> StartsWithQueryAsync(string propertyName, string givenValue)
        {
            return Task<IList<T>>.Factory.StartNew(() =>
            {
                var prop = typeof(T).GetProperties().FirstOrDefault(p => string.Equals(p.Name, propertyName));
                return store.Where(s => prop.GetValue(s).ToString().StartsWith(givenValue)).ToList(); ;
            });
        }

        public Task<IEnumerable<T>> QueryAsync(Func<T, bool> where)
        {
            return Task<IEnumerable<T>>.Factory.StartNew(() =>
            {
                if (where == null) return null;
                return this.store.Where(where);
            });
        }

        public Task<T> ReadAsync(string id, string orgId)
        {
            if (orgId == null)
            {
                return this.ReadAsync(id);
            }
            else
            {
                return Task<T>.Factory.StartNew(() =>
                {
                    if (id == null) return default(T);
                    var qualifiedId = id.AsQualifiedId(typeof(T));
                    return this.store.FirstOrDefault(e => string.Equals(e.Id, id) && string.Equals(e.OrganisationId, orgId));
                });
            }
        }

        public Task<IList<T>> ListAsync(string orgId)
        {
            return Task<IList<T>>.Factory.StartNew(() =>
           {
               return new ReadOnlyCollection<T>(this.store.Where(o => string.Equals(orgId, o.OrganisationId)).ToList());
           });
        }
    }
}