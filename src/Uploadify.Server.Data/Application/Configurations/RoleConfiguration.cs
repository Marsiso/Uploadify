using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uploadify.Server.Data.Infrastructure;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Data.Infrastructure.EF.Helpers;
using Uploadify.Server.Domain.Application.Models;

namespace Uploadify.Server.Data.Application.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable(Tables.Roles, Schemas.Application)
            .HasKey(entity => entity.Id);

        builder.HasIndex(entity => entity.NormalizedName).IsUnique();

        ChangeTrackingBaseEntityConfigurationHelpers<Role>.Configure(builder);

        builder.Property(entity => entity.Name)
            .IsUnicode(false)
            .IsRequired(false)
            .HasMaxLength(256);

        builder.Property(entity => entity.NormalizedName)
            .IsUnicode(false)
            .IsRequired(false)
            .HasMaxLength(256);

        builder.Property(entity => entity.Permission)
            .IsRequired();

        builder.HasMany(entity => entity.Claims)
            .WithOne()
            .HasForeignKey(entity => entity.RoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(entity => entity.Users)
            .WithOne(entity => entity.Role)
            .HasForeignKey(entity => entity.RoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
