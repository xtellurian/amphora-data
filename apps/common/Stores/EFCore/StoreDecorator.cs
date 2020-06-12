using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Amphora.Common.Contracts;

namespace Amphora.Common.Stores.EFCore
{
    public abstract class StoreDecorator<T> : IEntityStore<T> where T : class, IEntity
    {
        protected IEntityStore<T> store;
        public StoreDecorator(IEntityStore<T> store)
        {
            this.store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public virtual Task<int> CountAsync(Expression<Func<T, bool>>? where = null)
        {
            if (this.store != null)
            {
                return store.CountAsync();
            }
            else
            {
                throw new InvalidOperationException($"{nameof(store)} cannot be null");
            }
        }

        public virtual Task<T> CreateAsync(T entity)
        {
            if (this.store != null)
            {
                return store.CreateAsync(entity);
            }
            else
            {
                throw new InvalidOperationException($"{nameof(store)} cannot be null");
            }
        }

        public virtual Task DeleteAsync(T entity)
        {
            if (this.store != null)
            {
                return store.DeleteAsync(entity);
            }
            else
            {
                throw new InvalidOperationException($"{nameof(store)} cannot be null");
            }
        }

        public virtual bool IsModified<TProperty>(T model, Expression<Func<T, TProperty>> propertyExpression)
        {
            if (this.store != null)
            {
                return store.IsModified<TProperty>(model, propertyExpression);
            }
            else
            {
                throw new InvalidOperationException($"{nameof(store)} cannot be null");
            }
        }

        public virtual IQueryable<T> Query(Expression<Func<T, bool>> where)
        {
            if (this.store != null)
            {
                return store.Query(where);
            }
            else
            {
                throw new InvalidOperationException($"{nameof(store)} cannot be null");
            }
        }

        public virtual Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> where)
        {
            if (this.store != null)
            {
                return store.QueryAsync(where);
            }
            else
            {
                throw new InvalidOperationException($"{nameof(store)} cannot be null");
            }
        }

        public virtual Task<T?> ReadAsync(string id)
        {
            if (this.store != null)
            {
                return store.ReadAsync(id);
            }
            else
            {
                throw new InvalidOperationException($"{nameof(store)} cannot be null");
            }
        }

        public virtual Task<IList<T>> TopAsync()
        {
            if (this.store != null)
            {
                return store.TopAsync();
            }
            else
            {
                throw new InvalidOperationException($"{nameof(store)} cannot be null");
            }
        }

        public virtual Task<T> UpdateAsync(T entity)
        {
            if (this.store != null)
            {
                return store.UpdateAsync(entity);
            }
            else
            {
                throw new InvalidOperationException($"{nameof(store)} cannot be null");
            }
        }
    }
}