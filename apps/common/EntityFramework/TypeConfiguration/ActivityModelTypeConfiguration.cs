using Amphora.Common.Models.Activities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amphora.Common.EntityFramework.TypeConfiguration
{
    public class ActivityModelTypeConfiguration : IEntityTypeConfiguration<ActivityModel>
    {
        public void Configure(EntityTypeBuilder<ActivityModel> builder)
        {
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            // builder.HasIndex(p => p.Name);
            builder
                .HasOne(_ => _.Organisation)
                .WithMany()
                .HasForeignKey(_ => _.OrganisationId);
        }
    }
}