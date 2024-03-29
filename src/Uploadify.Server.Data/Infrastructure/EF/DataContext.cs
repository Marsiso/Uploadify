﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.EntityFrameworkCore.Models;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Data.Models;
using Uploadify.Server.Domain.Files.Models;
using File = Uploadify.Server.Domain.Files.Models.File;

namespace Uploadify.Server.Data.Infrastructure.EF;

public class DataContext : IdentityDbContext<User, Role, string, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
    private readonly ISaveChangesInterceptor _interceptor;

    public virtual DbSet<CodeList> CodeLists { get; set; } = null!;
    public virtual DbSet<CodeListItem> CodeListItems { get; set; } = null!;
    public virtual DbSet<Folder> Folders { get; set; } = null!;
    public virtual DbSet<File> Files { get; set; } = null!;

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        _interceptor = null!;
    }

    [ActivatorUtilitiesConstructor]
    public DataContext(DbContextOptions<DataContext> options, ISaveChangesInterceptor interceptor) : base(options)
    {
        _interceptor = interceptor;
    }

    public virtual Task<EntityEntry<TEntity>> UpdateEntity<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : BaseEntity
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(base.Update(entity));
    }

    public virtual Task<EntityEntry<TEntity>> DeleteEntity<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : BaseEntity
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(base.Remove(entity));
    }

    public virtual Task<EntityEntry<TEntity>> SoftDeleteEntity<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : BaseEntity
    {
        cancellationToken.ThrowIfCancellationRequested();
        entity.IsActive = false;

        return Task.FromResult(base.Update(entity));
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.AddInterceptors(_interceptor);

        optionsBuilder.UseOpenIddict();
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(Schemas.Application);

        modelBuilder.Entity<OpenIddictEntityFrameworkCoreToken>().ToTable(Tables.Tokens, Schemas.OpenIdConnect);
        modelBuilder.Entity<OpenIddictEntityFrameworkCoreApplication>().ToTable(Tables.Applications, Schemas.OpenIdConnect);
        modelBuilder.Entity<OpenIddictEntityFrameworkCoreAuthorization>().ToTable(Tables.Authorizations, Schemas.OpenIdConnect);
        modelBuilder.Entity<OpenIddictEntityFrameworkCoreScope>().ToTable(Tables.Scopes, Schemas.OpenIdConnect);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);

        // For testing purposes
        if (!Database.IsNpgsql())
        {
            modelBuilder.Entity<File>().Ignore(file => file.SearchVector);
        }
    }
}
