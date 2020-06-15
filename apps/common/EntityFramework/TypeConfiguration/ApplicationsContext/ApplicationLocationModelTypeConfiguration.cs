using Amphora.Common.Models.Applications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amphora.Common.EntityFramework.TypeConfiguration.ApplicationsContext
{
    public class ApplicationLocationModelTypeConfiguration : IEntityTypeConfiguration<ApplicationLocationModel>
    {
        public void Configure(EntityTypeBuilder<ApplicationLocationModel> builder)
        {
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.AllowedRedirectPaths).HasJsonConversion();
            builder.Property(p => p.PostLogoutRedirects).HasJsonConversion();
        }
    }
}