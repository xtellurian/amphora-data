using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.DataRequests;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Models.Platform;
using Amphora.Common.Models.Purchases;
using Amphora.Common.Models.Signals;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Amphora.Api.EntityFramework
{
    // DbContext is injected with a Scoped lifetime
    public class AmphoraContext : IdentityDbContext<ApplicationUser>
    {
        public AmphoraContext(DbContextOptions<AmphoraContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());

            modelBuilder.Entity<RestrictionModel>(b =>
            {
                b.HasKey(_ => new
                {
                    Source = _.SourceOrganisationId,
                    Target = _.TargetOrganisationId
                });
                b.HasOne(_ => _.SourceOrganisation).WithMany(_ => _.Restrictions).HasForeignKey(_ => _.SourceOrganisationId);
                b.HasOne(_ => _.TargetOrganisation).WithMany().HasForeignKey(_ => _.TargetOrganisationId);
            });

            modelBuilder.Entity<PurchaseModel>(b =>
            {
                b.Property(p => p.Id).ValueGeneratedOnAdd();
                b.HasOne(p => p.PurchasedByUser).WithMany(u => u.Purchases).HasForeignKey(p => p.PurchasedByUserId);
                b.HasOne(p => p.PurchasedByOrganisation).WithMany().HasForeignKey(p => p.PurchasedByOrganisationId);
                b.HasOne(p => p.Amphora).WithMany(u => u.Purchases).HasForeignKey(p => p.AmphoraId);
            });

            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.HasOne(p => p.Organisation).WithMany().HasForeignKey(p => p.OrganisationId);
                b.OwnsOne(_ => _.PinnedAmphorae, p =>
                {
                    p.HasOne(_ => _.Amphora1).WithMany().HasForeignKey(_ => _.AmphoraId1);
                    p.HasOne(_ => _.Amphora2).WithMany().HasForeignKey(_ => _.AmphoraId2);
                    p.HasOne(_ => _.Amphora3).WithMany().HasForeignKey(_ => _.AmphoraId3);
                    p.HasOne(_ => _.Amphora4).WithMany().HasForeignKey(_ => _.AmphoraId4);
                    p.HasOne(_ => _.Amphora5).WithMany().HasForeignKey(_ => _.AmphoraId5);
                    p.HasOne(_ => _.Amphora6).WithMany().HasForeignKey(_ => _.AmphoraId6);
                });
            });
        }

        public DbSet<AmphoraModel> Amphorae { get; set; }
        public DbSet<OrganisationModel> Organisations { get; set; }
        public DbSet<DataRequestModel> DataRequests { get; set; }
        public DbSet<PurchaseModel> Purchases { get; set; }
        public DbSet<CommissionModel> Commissions { get; set; }
        public DbSet<InvitationModel> Invitations { get; set; }
    }
}