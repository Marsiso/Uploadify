using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using Uploadify.Authorization.Models;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Application.Constants;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Authorization.Constants;
using Uploadify.Server.Domain.Infrastructure.Services;

namespace Uploadify.Server.Application.Infrastructure.Services;

public class Worker : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public Worker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        await MigrateDatabase(
            scope.ServiceProvider.GetRequiredService<DataContext>(),
            scope.ServiceProvider.GetRequiredService<SystemSettings>(),
            scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>());

        await SeedRolesTable(scope.ServiceProvider.GetRequiredService<RoleManager<Role>>());

        await SeedClientsTable(
            scope.ServiceProvider.GetRequiredService<SystemSettings>(),
            scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>());

        await SeedScopesTable(
            scope.ServiceProvider.GetRequiredService<SystemSettings>(),
            scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>());
    }

    private static async Task MigrateDatabase(DataContext context, SystemSettings settings, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment() && settings.Database.IsSeedEnabled)
        {
            await context.Database.EnsureDeletedAsync();
        }

        await context.Database.MigrateAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static async Task SeedScopesTable(SystemSettings settings, IOpenIddictScopeManager manager)
    {
        var api = new OpenIddictScopeDescriptor
        {
            DisplayName = settings.Api.DisplayName,
            Name = Scopes.Api,
            Resources =
            {
                settings.Api.ID
            }
        };

        var scope = await manager.FindByNameAsync(Scopes.Api);
        if (scope != null)
        {
            await manager.DeleteAsync(scope);
        }

        await manager.CreateAsync(api);
    }

    private static async Task SeedClientsTable(SystemSettings settings, IOpenIddictApplicationManager manager)
    {
        var api = new OpenIddictApplicationDescriptor
        {
            ClientId = settings.Api.ID,
            ClientSecret = settings.Api.Secret,
            DisplayName = settings.Api.DisplayName,
            ClientType = OpenIddictConstants.ClientTypes.Confidential,
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Introspection
            }
        };

        var application = await manager.FindByClientIdAsync(settings.Api.ID);
        if (application != null)
        {
            await manager.DeleteAsync(application);
        }

        await manager.CreateAsync(api);

        var client = new OpenIddictApplicationDescriptor
        {
            ClientId = settings.Client.ID,
            ClientSecret = settings.Client.Secret,
            DisplayName = settings.Client.DisplayName,
            ClientType = OpenIddictConstants.ClientTypes.Confidential,
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
            PostLogoutRedirectUris =
            {
                new Uri(settings.Client.PostLogoutRedirectUri)
            },
            RedirectUris =
            {
                new Uri(settings.Client.RedirectUri)
            },
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Logout,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                OpenIddictConstants.Permissions.Prefixes.Scope + Scopes.Name,
                OpenIddictConstants.Permissions.Prefixes.Scope + Scopes.Email,
                OpenIddictConstants.Permissions.Prefixes.Scope + Scopes.Phone,
                OpenIddictConstants.Permissions.Prefixes.Scope + Scopes.Roles,
                OpenIddictConstants.Permissions.Prefixes.Scope + Scopes.Api
            },
            Requirements =
            {
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
            }
        };

        application = await manager.FindByClientIdAsync(settings.Client.ID);
        if (application != null)
        {
            await manager.DeleteAsync(application);
        }

        await manager.CreateAsync(client);
    }

    private static async Task SeedRolesTable(RoleManager<Role> manager)
    {
        var role = await manager.FindByNameAsync(Roles.Defaults.SystemAdmin);
        if (role == null)
        {
            await manager.CreateAsync(new Role
            {
                Name = Roles.Defaults.SystemAdmin,
                Permission = Permission.All
            });
        }
        else
        {
            role.Name = Roles.Defaults.SystemAdmin;
            role.Permission = Permission.All;

            await manager.UpdateAsync(role);
        }

        role = await manager.FindByNameAsync(Roles.Defaults.UserAdmin);
        if (role == null)
        {
            await manager.CreateAsync(new Role
            {
                Name = Roles.Defaults.UserAdmin,
                Permission = Permission.ViewUsers | Permission.EditUsers | Permission.ViewFiles | Permission.EditFiles
            });
        }
        else
        {
            role.Name = Roles.Defaults.UserAdmin;
            role.Permission = Permission.ViewUsers | Permission.EditUsers | Permission.ViewFiles | Permission.EditFiles;

            await manager.UpdateAsync(role);
        }

        role = await manager.FindByNameAsync(Roles.Defaults.RoleAdmin);
        if (role == null)
        {
            await manager.CreateAsync(new Role
            {
                Name = Roles.Defaults.RoleAdmin,
                Permission = Permission.ViewRoles | Permission.EditRoles
            });
        }
        else
        {
            role.Name = Roles.Defaults.RoleAdmin;
            role.Permission = Permission.ViewRoles | Permission.EditRoles;

            await manager.UpdateAsync(role);
        }

        role = await manager.FindByNameAsync(Roles.Defaults.DefaultUser);
        if (role == null)
        {
            await manager.CreateAsync(new Role
            {
                Name = Roles.Defaults.DefaultUser,
                Permission = Permission.None
            });
        }
        else
        {
            role.Name = Roles.Defaults.DefaultUser;
            role.Permission = Permission.None;

            await manager.UpdateAsync(role);
        }
    }
}
