using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Data.Infrastructure.EF.Helpers;
using Uploadify.Server.Domain.Files.Models;

namespace Uploadify.Server.Data.Files.Configurations;

public class FileLinkConfiguration : IEntityTypeConfiguration<FileLink>
{
    public void Configure(EntityTypeBuilder<FileLink> builder)
    {
        builder.ToTable(Tables.FileLinks, Schemas.Files)
            .HasKey(entity => entity.Id);

        builder.HasIndex(entity => entity.UserId);
        builder.HasIndex(entity => entity.FileId);

        ChangeTrackingBaseEntityConfigurationHelpers<FileLink>.Configure(builder);
    }
}
