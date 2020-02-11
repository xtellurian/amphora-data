using Amphora.Common.Models.Purchases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amphora.Api.EntityFramework.TypeConfiguration
{
    public class CommissionModelTypeConfiguration : IEntityTypeConfiguration<CommissionModel>
    {
        public void Configure(EntityTypeBuilder<CommissionModel> builder)
        {
            builder.Property(_ => _.Id).ValueGeneratedOnAdd();
            builder.HasOne(_ => _.PurchaseModel).WithMany().HasForeignKey(_ => _.PurchaseModelId);
            builder.ToContainer("Platform");
        }
    }
}