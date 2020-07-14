using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace Amphora.Common.Stores.EFCore
{
    public abstract class EFStoreBase<T, TContext> where T : class, IEntity where TContext : DbContext
    {
        protected readonly ILogger<EFStoreBase<T, TContext>>? logger;
        protected readonly TContext context;
        private readonly Func<TContext, DbSet<T>> selectDbSet;

        public EFStoreBase(TContext context, Func<TContext, DbSet<T>> selectDbSet)
        {
            // only subscribe once
            context.ChangeTracker.StateChanged -= this.ChangeTracker_StateChanged;
            context.ChangeTracker.StateChanged += this.ChangeTracker_StateChanged;

            this.context = context;
            this.selectDbSet = selectDbSet;
        }

        public EFStoreBase(TContext context,
                           ILogger<EFStoreBase<T, TContext>> logger,
                           Func<TContext, DbSet<T>> entitySetExpression) : this(context, entitySetExpression)
        {
            this.logger = logger;
        }

        public bool IsModified<TProperty>(T model, Expression<Func<T, TProperty>> propertyExpression)
        {
            return this.context.Entry(model).Property(propertyExpression).IsModified;
        }

        private DbSet<T> Set => this.selectDbSet(context);
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? where = null)
        {
            if (where == null) { return await this.Set.CountAsync(); }
            else { return await Set.Where(where).CountAsync(); }
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            var addTask = await this.Set.AddAsync(entity);
            await context.SaveChangesAsync();
            return addTask.Entity;
        }

        public virtual async Task<T?> ReadAsync(string id)
        {
            var result = await Set.SingleOrDefaultAsync(a => a.Id == id);
            return result;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            OnUpdateEntity(entity as ITtl);
            await context.SaveChangesAsync();
            var existing = await Set.SingleOrDefaultAsync(a => a.Id == entity.Id);
            return existing;
        }

        public virtual async Task DeleteAsync(T entity)
        {
            var e = await this.Set.SingleOrDefaultAsync(a => a.Id == entity.Id);
            if (e != null)
            {
                this.Set.Remove(e);
                await context.SaveChangesAsync();
            }
        }

        public virtual IQueryable<T> Query(Expression<Func<T, bool>> where)
        {
            return Set.Where(where);
        }

        public virtual async Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> where, int skip, int take)
        {
            return await Set.Where(where)
                .OrderByDescending(_ => _.LastModified)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public virtual async Task<IList<T>> TopAsync()
        {
            return await Set.Take(10).ToListAsync();
        }

        private void ChangeTracker_StateChanged(object sender, EntityStateChangedEventArgs e)
        {
            if (e.Entry.Entity is IEntity && e.Entry.State == EntityState.Modified)
            {
                var entity = (IEntity)e.Entry.Entity;
                entity.LastModified = System.DateTime.UtcNow;
                logger.LogInformation($"Enitity {entity.Id} was modified");
            }
        }

        private void OnUpdateEntity(ITtl? entity)
        {
            if (entity != null && entity.ttl == null)
            {
                entity.ttl = -1;
                logger?.LogInformation($"Setting {entity.Id} ttl to -1");
            }
        }
    }
}
