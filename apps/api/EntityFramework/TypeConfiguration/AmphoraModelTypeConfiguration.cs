using Amphora.Common.Models.Amphorae;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amphora.Api.EntityFramework.TypeConfiguration
{
    public class AmphoraModelTypeConfiguration : IEntityTypeConfiguration<AmphoraModel>
    {
        public void Configure(EntityTypeBuilder<AmphoraModel> builder)
        {
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.HasOne(p => p.Organisation).WithMany().HasForeignKey(p => p.OrganisationId);
            builder.Property(e => e.GeoLocation).HasJsonConversion();
            builder.Property(e => e.FilesMetaData).HasJsonConversion();
            builder.OwnsMany(e => e.V2Signals, _ =>
            {
                _.Property(s => s.UUID).ValueGeneratedOnAdd();
                _.HasKey(s => s.UUID);
                _.Property(s => s.Meta).HasJsonConversion();
            });
            builder.OwnsMany(_ => _.Labels, _ =>
            {
                _.Property(label => label.Id).ValueGeneratedOnAdd();
                _.HasKey(label => new { id = label.Id, name = label.Name });
            });

            builder.HasMany(p => p.Purchases).WithOne(a => a.Amphora).HasForeignKey(a => a.AmphoraId);
            builder.HasOne(p => p.CreatedBy).WithMany().HasForeignKey(a => a.CreatedById);
            builder.HasMany(p => p.Signals).WithOne(p => p.Amphora).HasForeignKey(p => p.AmphoraId);
        }
    }
}