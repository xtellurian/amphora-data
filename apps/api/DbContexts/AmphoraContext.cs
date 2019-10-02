using Amphora.Common.Models.Users;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Transactions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
                b.HasMany(p => p.Transactions).WithOne(a => a.Amphora).HasForeignKey(a => a.AmphoraId);
                b.HasOne(p => p.CreatedBy).WithMany().HasForeignKey(a => a.CreatedById);

            });

            modelBuilder.Entity<OrganisationModel>(b =>
            {
                b.Property(p => p.Id).ValueGeneratedOnAdd();
                b.OwnsMany(b => b.Invitations, a =>
                {
                    a.HasKey(nameof(Invitation.TargetEmail));
                });
                b.OwnsMany(b => b.Memberships, a => a.HasKey(nameof(Membership.UserModelId)));
                b.HasOne(p => p.CreatedBy).WithMany().HasForeignKey(a => a.CreatedById);
            });

            modelBuilder.Entity<TransactionModel>(b =>
            {
                b.Property(p => p.Id).ValueGeneratedOnAdd();
                b.HasOne(p => p.User).WithMany(u => u.Transactions).HasForeignKey(p => p.UserId);
                b.HasOne(p => p.Amphora).WithMany(u => u.Transactions).HasForeignKey(p => p.AmphoraId);
            });

            modelBuilder.Entity<ApplicationUser>(b => {
                b.HasOne(p => p.Organisation).WithMany().HasForeignKey(p => p.OrganisationId);
            });
        }

        public DbSet<AmphoraModel> Amphorae { get; set; }
        public DbSet<OrganisationModel> Organisations { get; set; }
        public DbSet<TransactionModel> Transactions { get; set; }
        // public DbSet<UserModel> Users { get; set; }
    }
}