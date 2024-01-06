using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uploadify.Server.Domain.Infrastructure.Data.Contracts;

namespace Uploadify.Server.Data.Infrastructure.EF.Helpers;

public static class ChangeTrackingBaseEntityConfigurationHelpers<TEntity> where TEntity : class, IChangeTrackingBaseEntity
{
    public static void Configure(EntityTypeBuilder<TEntity> builder)
    {
        BaseEntityConfigurationHelpers<TEntity>.Configure(builder);

        builder.Property(entity => entity.DateCreated)
            .IsRequired();

        builder.Property(entity => entity.DateUpdated)
            .IsRequired();

        builder.HasOne(entity => entity.UserCreatedBy)
            .WithMany()
            .HasForeignKey(entity => entity.CreatedBy)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(entity => entity.UserUpdatedBy)
            .WithMany()
            .HasForeignKey(entity => entity.UpdatedBy)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
