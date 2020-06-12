using Amphora.Common.Models.DataRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amphora.Common.EntityFramework.TypeConfiguration.AmphoraContext
{
    public class DataRequestTypeConfiguration : IEntityTypeConfiguration<DataRequestModel>
    {
        public void Configure(EntityTypeBuilder<DataRequestModel> builder)
        {
            builder.Property(_ => _.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.GeoLocation).HasJsonConversion();
            builder.Property(e => e.UserIdVotes).HasJsonConversion();
            builder.HasOne(_ => _.CreatedBy).WithMany().HasForeignKey(_ => _.CreatedById);
        }
    }
}