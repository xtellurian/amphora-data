using Amphora.Common.Models.Activities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amphora.Common.EntityFramework.TypeConfiguration
{
    public class ActivityRunModelTypeConfiguration : IEntityTypeConfiguration<ActivityRunModel>
    {
        public void Configure(EntityTypeBuilder<ActivityRunModel> builder)
        {
            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder
                .HasOne(_ => _.Activity)
                .WithMany(_ => _!.Runs)
                .HasForeignKey(_ => _.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .OwnsMany(_ => _.AmphoraReferences, r =>
                {
                    r.Property(a => a.Id).ValueGeneratedOnAdd();
                    r.HasIndex(a => a.AmphoraId);
                    r
                        .HasOne(a => a.Amphora)
                        .WithMany()
                        .HasForeignKey(a => a.AmphoraId);
                });
        }
    }
}