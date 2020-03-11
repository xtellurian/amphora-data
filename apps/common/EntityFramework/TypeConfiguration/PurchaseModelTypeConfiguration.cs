using Amphora.Common.Models.Purchases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amphora.Common.EntityFramework.TypeConfiguration
{
    public class PurchaseModelTypeConfiguration : IEntityTypeConfiguration<PurchaseModel>
    {
        public void Configure(EntityTypeBuilder<PurchaseModel> builder)
        {
            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.HasOne(p => p.PurchasedByUser)
                .WithMany(u => u!.Purchases)
                .HasForeignKey(p => p.PurchasedByUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.PurchasedByOrganisation)
                .WithMany()
                .HasForeignKey(p => p.PurchasedByOrganisationId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(p => p.Amphora)
                .WithMany(u => u.Purchases)
                .HasForeignKey(p => p.AmphoraId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}