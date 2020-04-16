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
    public class AmphoraContext : DbContext
    {
        public AmphoraContext(DbContextOptions<AmphoraContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Amphora.Common.EntityFramework.TypeConfiguration.AmphoraModelTypeConfiguration).Assembly);
        }

        public DbSet<AmphoraAccessControlModel> AmphoraAccessControls { get; set; }
        public DbSet<AmphoraModel> Amphorae { get; set; } = null!;
        public DbSet<OrganisationModel> Organisations { get; set; } = null!;
        public DbSet<DataRequestModel> DataRequests { get; set; } = null!;
        public DbSet<PurchaseModel> Purchases { get; set; } = null!;
        public DbSet<CommissionModel> Commissions { get; set; } = null!;
        public DbSet<InvitationModel> Invitations { get; set; } = null!;
        public DbSet<ApplicationUserDataModel> UserData { get; set; }
    }
}