using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Data.Infrastructure.EF.Helpers;
using Uploadify.Server.Domain.Files.Models;

namespace Uploadify.Server.Data.Files.Configurations;

public class SharedFileConfiguration : IEntityTypeConfiguration<SharedFile>
{
    public void Configure(EntityTypeBuilder<SharedFile> builder)
    {
        builder.ToTable(Tables.SharedFiles, Schemas.FileSystem)
            .HasKey(entity => entity.Id);

        builder.HasIndex(entity => entity.UserId);
        builder.HasIndex(entity => entity.FileId);

        ChangeTrackingBaseEntityConfigurationHelpers<SharedFile>.Configure(builder);
    }
}
