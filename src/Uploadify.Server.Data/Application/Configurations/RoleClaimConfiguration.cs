using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uploadify.Server.Data.Infrastructure;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Data.Infrastructure.EF.Helpers;
using Uploadify.Server.Domain.Application.Models;

namespace Uploadify.Server.Data.Application.Configurations;

public class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
{
    public void Configure(EntityTypeBuilder<RoleClaim> builder)
    {
        builder.ToTable(Tables.RoleClaims, Schemas.Application)
            .HasKey(entity => entity.Id);

        BaseEntityConfigurationHelpers<RoleClaim>.Configure(builder);

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
