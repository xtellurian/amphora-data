using Amphora.Common.Models.Applications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amphora.Common.EntityFramework.TypeConfiguration.ApplicationsContext
{
    public class ApplicationModelTypeConfiguration : IEntityTypeConfiguration<ApplicationModel>
    {
        public void Configure(EntityTypeBuilder<ApplicationModel> builder)
        {
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.HasMany(p => p.Locations).WithOne().HasForeignKey(_ => _.ApplicationId);
        }
    }
}