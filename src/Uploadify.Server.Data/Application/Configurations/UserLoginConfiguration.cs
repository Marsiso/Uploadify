using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uploadify.Server.Data.Infrastructure;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Data.Infrastructure.EF.Helpers;
using Uploadify.Server.Domain.Application.Models;

namespace Uploadify.Server.Data.Application.Configurations;

public class UserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
{
    public void Configure(EntityTypeBuilder<UserLogin> builder)
    {
        builder.ToTable(Tables.UserLogins, Schemas.Application)
            .HasKey(entity => new { entity.LoginProvider, entity.ProviderKey });

        BaseEntityConfigurationHelpers<UserLogin>.Configure(builder);

        builder.Property(entity => entity.LoginProvider)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(256);

        builder.Property(entity => entity.ProviderKey)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(256);
    }
}
