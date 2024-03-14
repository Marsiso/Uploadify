using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Data.Infrastructure.EF.Helpers;
using File = Uploadify.Server.Domain.Files.Models.File;

namespace Uploadify.Server.Data.Files.Configurations;

public class FileConfiguration : IEntityTypeConfiguration<File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        builder.ToTable(Tables.Files, Schemas.Files)
            .HasKey(entity => entity.Id);

        builder.HasIndex(entity => entity.FolderId);
        builder.HasIndex(entity => entity.CategoryId);
        builder.HasIndex(entity => entity.UnsafeName);
        builder.HasIndex(entity => entity.IsPublic);

        ChangeTrackingBaseEntityConfigurationHelpers<File>.Configure(builder);

        builder.Property(entity => entity.SafeName)
            .HasMaxLength(256)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(entity => entity.UnsafeName)
            .HasMaxLength(256)
            .IsUnicode()
            .IsRequired();

        builder.Property(entity => entity.Extension)
            .HasMaxLength(256)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(entity => entity.MimeType)
            .HasMaxLength(256)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(entity => entity.Location)
            .HasMaxLength(1024)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(entity => entity.Size)
            .IsRequired();

        builder.HasGeneratedTsVectorColumn(
                entity => entity.SearchVector,
                "english",
                entity => new { entity.UnsafeName, entity.Extension, entity.MimeType })
            .HasIndex(entity => entity.SearchVector)
            .HasMethod("GIN");

        builder.HasOne(entity => entity.Category)
            .WithMany()
            .HasForeignKey(entity => entity.CategoryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
