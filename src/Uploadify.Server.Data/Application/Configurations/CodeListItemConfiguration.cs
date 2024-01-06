using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Data.Infrastructure.EF.Helpers;
using Uploadify.Server.Domain.Application.Models;

namespace Uploadify.Server.Data.Application.Configurations;

public class CodeListItemConfiguration : IEntityTypeConfiguration<CodeListItem>
{
    public void Configure(EntityTypeBuilder<CodeListItem> builder)
    {
        builder.ToTable(Tables.CodeListItems, Schemas.Application)
            .HasKey(entity => entity.Id);

        builder.HasIndex(entity => entity.CodeListId);

        BaseEntityConfigurationHelpers<CodeListItem>.Configure(builder);

        builder.Property(entity => entity.Value)
            .IsUnicode()
            .IsRequired()
            .HasMaxLength(256);
    }
}
