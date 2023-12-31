using Amphora.Common.EntityFramework.TypeConfiguration.AmphoraContext;
using Amphora.Common.Models.Activities;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.DataRequests;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
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
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AmphoraModelTypeConfiguration).Assembly,
            _ => _.Namespace.StartsWith(typeof(AmphoraModelTypeConfiguration).Namespace));
        }

        public DbSet<AmphoraAccessControlModel> AmphoraAccessControls { get; set; }
        public DbSet<AmphoraModel> Amphorae { get; set; } = null!;
        public DbSet<ActivityModel> Activities { get; set; } = null!;
        public DbSet<ActivityRunModel> ActivityRuns { get; set; } = null!;
        public DbSet<InvoiceModel> Invoices { get; set; } = null!;
        public DbSet<OrganisationModel> Organisations { get; set; } = null!;
        public DbSet<TermsOfUseModel> TermsOfUse { get; set; }
        public DbSet<DataRequestModel> DataRequests { get; set; } = null!;
        public DbSet<PurchaseModel> Purchases { get; set; } = null!;
        public DbSet<CommissionModel> Commissions { get; set; } = null!;
        public DbSet<InvitationModel> Invitations { get; set; } = null!;
        public DbSet<ApplicationUserDataModel> UserData { get; set; }
    }
}