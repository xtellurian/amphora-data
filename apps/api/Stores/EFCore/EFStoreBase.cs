using Amphora.Api.DbContexts;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public abstract class EFStoreBase
    {
        protected readonly ILogger<EFStoreBase> logger;
        protected readonly AmphoraContext context;

        public EFStoreBase(AmphoraContext context)
        {
            // only subscribe once
            context.ChangeTracker.StateChanged -= this.ChangeTracker_StateChanged;
            context.ChangeTracker.StateChanged += this.ChangeTracker_StateChanged;

            this.context = context;
        }
        public EFStoreBase(AmphoraContext context, ILogger<EFStoreBase> logger): this(context)
        {
            this.logger = logger;
        }

        private void ChangeTracker_StateChanged(object sender, EntityStateChangedEventArgs e)
        {
            if (e.Entry.Entity is IEntity && e.Entry.State == EntityState.Modified)
            {
                var entity = ((IEntity)e.Entry.Entity);
                entity.LastModified = System.DateTime.UtcNow;
                logger.LogInformation($"Enitity {entity.Id} was modified");
            }
        }
        public virtual void OnUpdateEntity(Entity entity)
        {
            if (entity.ttl == null)
            {
                entity.ttl = -1;
                logger?.LogInformation($"Setting {entity.Id} ttl to -1");
            }
        }
    }
}
