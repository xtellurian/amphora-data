using Amphora.Common.Models;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public abstract class EFStoreBase
    {
        private readonly ILogger<EFStoreBase> logger;

        public EFStoreBase() {}
        public EFStoreBase(ILogger<EFStoreBase> logger)
        {
            this.logger = logger;
        }
        public virtual void OnUpdateEntity(Entity entity)
        {
            if(entity.ttl == null) 
            {
                entity.ttl = -1;
                logger?.LogInformation($"Setting {entity.Id} ttl to -1");
            }
        }

    }
}