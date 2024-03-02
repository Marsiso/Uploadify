using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uploadify.Server.Domain.Data.Contracts;

namespace Uploadify.Server.Data.Infrastructure.EF.Helpers;

public static class BaseEntityConfigurationHelpers<TEntity> where TEntity : class, IBaseEntity
{
    public static void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasIndex(entity => entity.IsActive);
        builder.HasQueryFilter(entity => entity.IsActive);
    }
}
