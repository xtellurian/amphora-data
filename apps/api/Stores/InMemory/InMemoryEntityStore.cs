using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Contracts;
using AutoMapper;

namespace Amphora.Api.Stores
{
    public class InMemoryEntityStore<T> : IEntityStore<T> where T : class, IEntity
    {
        public InMemoryEntityStore(IMapper mapper)
        {
            this.mapper = mapper;
        }
        protected List<T> store = new List<T>();
        private readonly IMapper mapper;

        public Task<IList<T>> TopAsync()
        {
            return Task<IList<T>>.Factory.StartNew(() =>
            {
                return new ReadOnlyCollection<T>(this.store);
            });
        }
         public Task<IList<T>> TopAsync(string orgId)
        {
            return Task<IList<T>>.Factory.StartNew(() =>
           {
               return new ReadOnlyCollection<T>(this.store.Where(o => string.Equals(orgId, o.OrganisationId)).ToList());
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
        public async Task<TExtended> CreateAsync<TExtended>(TExtended entity) where TExtended : class, T
        {
            return (TExtended)await this.CreateAsync(entity as T);
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

        public Task<IList<TExtended>> StartsWithQueryAsync<TExtended>(string propertyName, string givenValue) where TExtended : class, T
        {
            return Task<IList<TExtended>>.Factory.StartNew(() =>
            {
                var prop = typeof(TExtended).GetProperties().FirstOrDefault(p => string.Equals(p.Name, propertyName));
                var q = store.Where(s => s is TExtended && prop.GetValue(s as TExtended).ToString().StartsWith(givenValue)).ToList();
                return mapper.Map<List<TExtended>>(q);
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
                    id = id.AsQualifiedId(typeof(T));
                    return this.store.FirstOrDefault(e => string.Equals(e.Id, id) && string.Equals(e.OrganisationId, orgId));
                });
            }
        }
        public async Task<TExtended> ReadAsync<TExtended>(string id) where TExtended : class, T
        {
            var entity = await this.ReadAsync(id);
            if (entity is TExtended) return entity as TExtended;
            else return mapper.Map<TExtended>(entity);
        }

        public async Task<TExtended> ReadAsync<TExtended>(string id, string orgId) where TExtended : class, T
        {
            var entity = await ReadAsync(id, orgId);
            if (entity is TExtended) return entity as TExtended;
            else return mapper.Map<TExtended>(entity);
        }


        public Task<IEnumerable<TQuery>> QueryAsync<TQuery>(Func<TQuery, bool> where) where TQuery : class, T
        {
            return Task<IEnumerable<TQuery>>.Factory.StartNew(() =>
            {
                if (where == null) return null;
                var results = new List<TQuery>();
                foreach(var s in store)
                {
                    if(s is TQuery && where(s as TQuery))
                    {
                        results.Add(s as TQuery);
                    }
                    else
                    {
                        var e = mapper.Map<TQuery>(s);
                        if(where(e)) results.Add(e);
                    }
                }
                
                return results;
            });
        }
    }
}