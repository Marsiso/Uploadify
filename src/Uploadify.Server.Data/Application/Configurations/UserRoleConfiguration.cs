using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Data.Infrastructure.EF.Helpers;
using Uploadify.Server.Domain.Application.Models;

namespace Uploadify.Server.Data.Application.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable(Tables.UserRoles, Schemas.Application)
            .HasKey(entity => new { entity.UserId, entity.RoleId });

        ChangeTrackingBaseEntityConfigurationHelpers<UserRole>.Configure(builder);
    }
}
