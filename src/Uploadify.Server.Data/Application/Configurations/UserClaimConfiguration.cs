using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Data.Infrastructure.EF.Helpers;
using Uploadify.Server.Domain.Application.Models;

namespace Uploadify.Server.Data.Application.Configurations;

public class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
{
    public void Configure(EntityTypeBuilder<UserClaim> builder)
    {
        builder.ToTable(Tables.UserClaims, Schemas.Application)
            .HasKey(entity => entity.Id);

        BaseEntityConfigurationHelpers<UserClaim>.Configure(builder);

        builder.Property(entity => entity.ClaimType)
            .IsUnicode(false)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(entity => entity.ClaimValue)
            .IsUnicode()
            .IsRequired()
            .HasMaxLength(256);
    }
}
