using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Data.Infrastructure.EF.Helpers;
using Uploadify.Server.Domain.FileSystem.Models;

namespace Uploadify.Server.Data.FileSystem.Configurations;

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
