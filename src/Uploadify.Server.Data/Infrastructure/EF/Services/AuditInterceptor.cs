﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Uploadify.Server.Domain.Data.Contracts;

namespace Uploadify.Server.Data.Infrastructure.EF.Services;

public class AuditInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not DataContext context)
        {
            return base.SavingChanges(eventData, result);
        }

        OnBeforeSavedChanges(context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not DataContext context)
        {
            return new(result);
        }

        OnBeforeSavedChanges(context);

        return new(result);
    }

    private static void OnBeforeSavedChanges(DataContext context)
    {
        context.ChangeTracker.DetectChanges();

        foreach (var entry in context.ChangeTracker.Entries<IBaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.IsActive = true;
            }
        }

        foreach (var entry in context.ChangeTracker.Entries<IChangeTrackingBaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.DateCreated = entry.Entity.DateUpdated = DateTime.UtcNow;
                continue;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.DateUpdated = DateTime.UtcNow;
            }
        }
    }
}
