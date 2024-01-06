using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Data.Infrastructure.EF.Helpers;
using Uploadify.Server.Domain.Files.Models;

namespace Uploadify.Server.Data.Files.Configurations;

public class FolderConfiguration : IEntityTypeConfiguration<Folder>
{
    public void Configure(EntityTypeBuilder<Folder> builder)
    {
        builder.ToTable(Tables.Folders, Schemas.Files)
            .HasKey(entity => entity.Id);

        builder.HasIndex(entity => entity.Name);
        builder.HasIndex(entity => entity.UserId);

        ChangeTrackingBaseEntityConfigurationHelpers<Folder>.Configure(builder);

        builder.Property(entity => entity.Name)
            .HasMaxLength(256)
            .IsUnicode()
            .IsRequired();

        builder.Property(entity => entity.TotalSize)
            .IsRequired();

        builder.Property(entity => entity.TotalCount)
            .IsRequired();

        builder.HasOne(entity => entity.User)
            .WithMany(entity => entity.Folders)
            .HasForeignKey(entity => entity.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(entity => entity.Category)
            .WithMany()
            .HasForeignKey(entity => entity.CategoryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(entity => entity.Files)
            .WithOne(entity => entity.Folder)
            .HasForeignKey(file => file.FolderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(entity => entity.Children)
            .WithOne(entity => entity.Parent)
            .HasForeignKey(entity => entity.ParentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
