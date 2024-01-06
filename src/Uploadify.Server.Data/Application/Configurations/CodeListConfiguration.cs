using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Data.Infrastructure.EF.Helpers;
using Uploadify.Server.Domain.Application.Models;

namespace Uploadify.Server.Data.Application.Configurations;

public class CodeListConfiguration : IEntityTypeConfiguration<CodeList>
{
    public void Configure(EntityTypeBuilder<CodeList> builder)
    {
        builder.ToTable(Tables.CodeLists, Schemas.Application)
            .HasKey(entity => entity.Id);

        builder.HasIndex(entity => entity.Name).IsUnique();

        BaseEntityConfigurationHelpers<CodeList>.Configure(builder);

        builder.Property(entity => entity.Name)
            .IsUnicode()
            .IsRequired()
            .HasMaxLength(256);

        builder.HasMany(entity => entity.CodeListItems)
            .WithOne()
            .HasForeignKey(entity => entity.CodeListId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
