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
using Newtonsoft.Json;

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

            modelBuilder.Entity<AmphoraSignalModel>(b =>
            {
                b.HasKey(c => new { c.AmphoraId, c.SignalId });
                b.HasOne(p => p.Amphora).WithMany(p => p.Signals).HasForeignKey(p => p.AmphoraId);
                b.HasOne(p => p.Signal).WithMany().HasForeignKey(p => p.SignalId);
            });

            modelBuilder.Entity<SignalModel>(b =>
            {
                b.HasKey(_ => _.Id);
            });

            modelBuilder.Entity<OrganisationModel>(organisation =>
            {
                organisation.Property(p => p.Id).ValueGeneratedOnAdd();
                // new global invitations
                organisation.HasMany(_ => _.GlobalInvitations).WithOne(_ => _.TargetOrganisation).HasForeignKey(_ => _.TargetOrganisationId);
                organisation.OwnsMany(b => b.Memberships, membership =>
                {
                    membership.HasKey(nameof(Membership.UserId));
                    membership.HasOne(b => b.User).WithMany().HasForeignKey(u => u.UserId);
                });
                organisation.HasOne(_ => _.CreatedBy).WithMany().HasForeignKey(a => a.CreatedById);
                organisation.OwnsMany(_ => _.TermsAndConditions, termsAndConditions =>
                {
                    termsAndConditions.WithOwner(_ => _.Organisation).HasForeignKey(_ => _.OrganisationId);
                    termsAndConditions.HasKey(_ => _.Id);
                });
                organisation.OwnsMany(p => p.TermsAndConditionsAccepted, a =>
                {
                    a.WithOwner(b => b.AcceptedByOrganisation).HasForeignKey(_ => _.AcceptedByOrganisationId);
                    a.HasOne(b => b.TermsAndConditionsOrganisation).WithMany().HasForeignKey(b => b.TermsAndConditionsOrganisationId);
                    a.HasKey(_ => new { _.TermsAndConditionsId, _.TermsAndConditionsOrganisationId }); // dual key
                });
                // Pinned Amphorae
                organisation.OwnsOne(_ => _.PinnedAmphorae, p =>
                {
                    p.HasOne(_ => _.Amphora1).WithMany().HasForeignKey(_ => _.AmphoraId1);
                    p.HasOne(_ => _.Amphora2).WithMany().HasForeignKey(_ => _.AmphoraId2);
                    p.HasOne(_ => _.Amphora3).WithMany().HasForeignKey(_ => _.AmphoraId3);
                    p.HasOne(_ => _.Amphora4).WithMany().HasForeignKey(_ => _.AmphoraId4);
                    p.HasOne(_ => _.Amphora5).WithMany().HasForeignKey(_ => _.AmphoraId5);
                    p.HasOne(_ => _.Amphora6).WithMany().HasForeignKey(_ => _.AmphoraId6);
                });
                // Account
                organisation.OwnsOne(_ => _.Account, account =>
                {
                    account.WithOwner(_ => _.Organisation).HasForeignKey(_ => _.OrganisationId);
                    account.OwnsMany(_ => _.Credits, credit =>
                    {
                        credit.Property(_ => _.Id).ValueGeneratedOnAdd();
                        credit.HasKey(_ => _.Id);
                        credit.WithOwner(_ => _.Account);
                    });
                    account.OwnsMany(_ => _.Debits, debit =>
                    {
                        debit.Property(_ => _.Id).ValueGeneratedOnAdd();
                        debit.HasKey(_ => _.Id);
                        debit.WithOwner(_ => _.Account);
                    });
                    account.OwnsMany(b => b.Invoices, invoice =>
                    {
                        invoice.Property(_ => _.Id).ValueGeneratedOnAdd();
                        invoice.OwnsMany(_ => _.Transactions, transaction =>
                        {
                            transaction.Property(_ => _.Id).ValueGeneratedOnAdd();
                            transaction.HasKey(_ => _.Id);
                        });
                        invoice.HasKey(_ => _.Id);
                        invoice.WithOwner(_ => _.Account);
                    });
                });
                organisation.HasMany(_ => _.Purchases).WithOne(p => p.PurchasedByOrganisation).HasForeignKey(_ => _.PurchasedByOrganisationId);
            });

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

            modelBuilder.Entity<InvitationModel>(b =>
            {
                b.Property(p => p.Id).ValueGeneratedOnAdd();
                b.HasOne(_ => _.TargetOrganisation).WithMany(_ => _.GlobalInvitations).HasForeignKey(_ => _.TargetOrganisationId);
                b.HasData(new InvitationModel { TargetDomain = "AMPHORADATA.COM", Id = "01", IsGlobalAdmin = true });
            });
        }

        public DbSet<AmphoraModel> Amphorae { get; set; }
        public DbSet<OrganisationModel> Organisations { get; set; }
        public DbSet<DataRequestModel> DataRequests { get; set; }
        public DbSet<PurchaseModel> Purchases { get; set; }
        public DbSet<CommissionModel> Commissions { get; set; }
        public DbSet<SignalModel> Signals { get; set; } // TODO: remove from context
        public DbSet<InvitationModel> Invitations { get; set; }
    }
}