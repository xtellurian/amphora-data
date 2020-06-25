using Amphora.Common.Models.Amphorae;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amphora.Common.EntityFramework.TypeConfiguration.AmphoraContext
{
    public class AmphoraModelTypeConfiguration : IEntityTypeConfiguration<AmphoraModel>
    {
        public void Configure(EntityTypeBuilder<AmphoraModel> builder)
        {
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.HasOne(p => p.Organisation).WithMany().HasForeignKey(p => p.OrganisationId);
            builder.Property(e => e.GeoLocation).HasJsonConversion();
            builder.Property(e => e.Quality).HasJsonConversion();
            builder.OwnsMany(e => e.V2Signals, _ =>
            {
                _.Property(s => s.UUID).ValueGeneratedOnAdd();
                _.HasKey(s => s.UUID);
                _.Property(s => s.Attributes).HasJsonConversion();
            });
            builder.OwnsMany(_ => _.Labels, _ =>
            {
                _.Property(label => label.Id).ValueGeneratedOnAdd();
                _.HasKey(label => new { id = label.Id, name = label.Name });
            });

            builder.HasOne(p => p.CreatedBy).WithMany().HasForeignKey(a => a.CreatedById);

            builder.HasOne(_ => _.AccessControl)
                .WithOne(_ => _.Amphora)
                .HasForeignKey<AmphoraAccessControlModel>(_ => _.AmphoraId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}