using Amphora.Common.Models.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amphora.Common.EntityFramework.TypeConfiguration
{
    public class RestrictionModelTypeConfiguration : IEntityTypeConfiguration<RestrictionModel>
    {
        public void Configure(EntityTypeBuilder<RestrictionModel> builder)
        {
            builder.HasKey(_ => _.Id);
            builder.Property(_ => _.Id).ValueGeneratedOnAdd();

            builder.HasOne(_ => _.SourceOrganisation)
                .WithMany(_ => _!.Restrictions)
                .HasForeignKey(_ => _.SourceOrganisationId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(_ => _.SourceAmphora)
                .WithMany(_ => _!.Restrictions)
                .HasForeignKey(_ => _.SourceAmphoraId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(_ => _.TargetOrganisation)
                .WithMany(_ => _!.TargetedByRestrictions)
                .HasForeignKey(_ => _.TargetOrganisationId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}