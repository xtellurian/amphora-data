using Amphora.Common.Models.Amphorae;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amphora.Common.EntityFramework.TypeConfiguration.AmphoraContext
{
    public class TermsOfUseModelTypeConfiguration : IEntityTypeConfiguration<TermsOfUseModel>
    {
        public void Configure(EntityTypeBuilder<TermsOfUseModel> builder)
        {
            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.HasOne(_ => _.Organisation)
                .WithMany(_ => _!.TermsOfUses)
                .HasForeignKey(_ => _.OrganisationId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(_ => _.AppliedToAmphoras)
                .WithOne(_ => _.TermsOfUse!)
                .HasForeignKey(_ => _.TermsOfUseId)
                .IsRequired(false);
        }
    }
}