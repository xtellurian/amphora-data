using Amphora.Common.EntityFramework;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.DataRequests;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Models.Platform;
using Amphora.Common.Models.Purchases;
using Amphora.Common.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace Amphora.Api.EntityFramework
{
    // DbContext is injected with a Scoped lifetime
    public class AmphoraContext : CommonContext
    {
        public AmphoraContext(DbContextOptions<AmphoraContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Amphora.Common.EntityFramework.CommonContext).Assembly);
        }
    }
}