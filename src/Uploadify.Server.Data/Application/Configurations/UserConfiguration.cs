using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Data.Infrastructure.EF.Helpers;
using Uploadify.Server.Domain.Application.Models;

namespace Uploadify.Server.Data.Application.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(Tables.Users, Schemas.Application)
            .HasKey(entity => entity.Id);

        builder.HasIndex(entity => entity.NormalizedUserName).IsUnique();
        builder.HasIndex(entity => entity.NormalizedEmail).IsUnique();

        ChangeTrackingBaseEntityConfigurationHelpers<User>.Configure(builder);

        builder.Property(entity => entity.UserName)
            .IsUnicode()
            .IsRequired(false)
            .HasMaxLength(256);

        builder.Property(entity => entity.NormalizedUserName)
            .IsUnicode(false)
            .IsRequired(false)
            .HasMaxLength(256);

        builder.Property(entity => entity.Email)
            .IsUnicode(false)
            .IsRequired(false)
            .HasMaxLength(256);

        builder.Property(entity => entity.NormalizedEmail)
            .IsUnicode(false)
            .IsRequired(false)
            .HasMaxLength(256);

        builder.Property(entity => entity.PhoneNumber)
            .IsUnicode(false)
            .IsRequired(false)
            .HasMaxLength(256);

        builder.Property(entity => entity.GivenName)
            .IsUnicode()
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(entity => entity.FamilyName)
            .IsUnicode()
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(entity => entity.Picture)
            .IsUnicode(false)
            .IsRequired(false)
            .HasMaxLength(4096);

        builder.Property(entity => entity.SecurityStamp)
            .IsUnicode(false)
            .IsRequired(false)
            .HasMaxLength(512);

        builder.Property(entity => entity.ConcurrencyStamp)
            .IsUnicode(false)
            .IsRequired(false)
            .HasMaxLength(512);

        builder.Property(entity => entity.PasswordHash)
            .IsUnicode(false)
            .IsRequired(false)
            .HasMaxLength(512);

        builder.HasMany(entity => entity.Claims)
            .WithOne()
            .HasForeignKey(entity => entity.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(entity => entity.Logins)
            .WithOne()
            .HasForeignKey(entity => entity.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(entity => entity.Tokens)
            .WithOne()
            .HasForeignKey(entity => entity.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(entity => entity.Roles)
            .WithOne(entity => entity.User)
            .HasForeignKey(entity => entity.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(entity => entity.SharedFiles)
            .WithOne(entity => entity.User)
            .HasForeignKey(entity => entity.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
