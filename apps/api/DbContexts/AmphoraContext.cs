using Amphora.Common.Models.Users;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Purchases;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Amphora.Common.Models.Signals;

namespace Amphora.Api.DbContexts
{

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
                b.OwnsMany(b => b.Invitations, a =>
                {
                    a.HasKey(nameof(Invitation.TargetEmail));
                });
                b.OwnsMany(b => b.Memberships, a =>
                {
                    a.HasKey(nameof(Membership.UserId));
                    a.HasOne(b => b.User).WithMany().HasForeignKey(u => u.UserId);
                });
                b.HasOne(p => p.CreatedBy).WithMany().HasForeignKey(a => a.CreatedById);
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
        }

        public DbSet<AmphoraModel> Amphorae { get; set; }
        public DbSet<OrganisationModel> Organisations { get; set; }
        public DbSet<PurchaseModel> Purchases { get; set; }
        public DbSet<SignalModel> Signals { get; set; }
    }
}