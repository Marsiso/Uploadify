using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Data.Infrastructure.EF.Helpers;
using Uploadify.Server.Domain.Application.Models;

namespace Uploadify.Server.Data.Application.Configurations;

public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.ToTable(Tables.UserTokens, Schemas.Application)
            .HasKey(entity => new { entity.UserId, entity.LoginProvider, entity.Name });

        BaseEntityConfigurationHelpers<UserToken>.Configure(builder);

        builder.Property(entity => entity.LoginProvider)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(256);

        builder.Property(entity => entity.Name)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(256);
    }
}
