using Amphora.Common.Models.Users;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Purchases;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Amphora.Common.Models.Signals;
using Amphora.Common.Models.Platform;
using Amphora.Common.Models.Permissions;

namespace Amphora.Api.DbContexts
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

            modelBuilder.Entity<AmphoraModel>(b =>
            {
                b.Property(p => p.Id).ValueGeneratedOnAdd();
                b.HasOne(p => p.Organisation).WithMany().HasForeignKey(p => p.OrganisationId);
                b.Property(e => e.GeoLocation)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<GeoLocation>(v));
                b.HasMany(p => p.Purchases).WithOne(a => a.Amphora).HasForeignKey(a => a.AmphoraId);
                b.HasOne(p => p.CreatedBy).WithMany().HasForeignKey(a => a.CreatedById);
                b.HasMany(p => p.Signals).WithOne(p => p.Amphora).HasForeignKey(p => p.AmphoraId);
            });

            modelBuilder.Entity<AmphoraSignalModel>(b =>
            {
                b.HasKey(c => new { c.AmphoraId, c.SignalId });
                b.HasOne(p => p.Amphora).WithMany(p => p.Signals).HasForeignKey(p => p.AmphoraId);
                b.HasOne(p => p.Signal).WithMany().HasForeignKey(p => p.SignalId);
            });


            modelBuilder.Entity<SignalModel>(b =>
            {
                b.HasData(new SignalModel("id", SignalModel.Numeric));
            });

            modelBuilder.Entity<OrganisationModel>(b =>
            {
                b.Property(p => p.Id).ValueGeneratedOnAdd();
                // new global invitations
                b.HasMany(_ => _.GlobalInvitations).WithOne(_ => _.TargetOrganisation).HasForeignKey(_ => _.TargetOrganisationId);
                b.OwnsMany(b => b.Memberships, a =>
                {
                    a.HasKey(nameof(Membership.UserId));
                    a.HasOne(b => b.User).WithMany().HasForeignKey(u => u.UserId);
                });
                b.HasOne(p => p.CreatedBy).WithMany().HasForeignKey(a => a.CreatedById);
                b.OwnsMany(p => p.TermsAndConditions, a =>
                {
                    a.WithOwner(b => b.Organisation).HasForeignKey(_ => _.OrganisationId);
                    a.HasKey(_ => _.Id);
                });
                b.OwnsMany(p => p.TermsAndConditionsAccepted, a =>
                {
                    a.WithOwner(b => b.AcceptedByOrganisation).HasForeignKey(_ => _.AcceptedByOrganisationId);
                    a.HasOne(b => b.TermsAndConditionsOrganisation).WithMany().HasForeignKey(b => b.TermsAndConditionsOrganisationId);
                    a.HasKey(_ => new { _.TermsAndConditionsId, _.TermsAndConditionsOrganisationId }); // dual key
                });
                b.OwnsMany(p => p.Restrictions, a =>
                {
                    a.HasKey(_ => _.TargetOrganisationId);
                });
                b.OwnsOne(_ => _.Account, a =>
                {
                    a.OwnsMany(b => b.Credits, c =>
                    {
                        c.Property(p => p.Id).ValueGeneratedOnAdd();
                        c.HasKey(d => d.Id);
                        c.WithOwner(d => d.Account);
                    });
                    a.OwnsMany(b => b.Debits, c =>
                    {
                        c.Property(p => p.Id).ValueGeneratedOnAdd();
                        c.HasKey(d => d.Id);
                        c.WithOwner(d => d.Account);
                    });
                });
                b.HasMany(_ => _.Purchases).WithOne(p => p.PurchasedByOrganisation).HasForeignKey(_ => _.PurchasedByOrganisationId);
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
        public DbSet<PurchaseModel> Purchases { get; set; }
        public DbSet<SignalModel> Signals { get; set; }
        public DbSet<InvitationModel> Invitations { get; set; }
    }
}