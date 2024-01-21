using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Uploadify.Authorization.Services;

namespace Uploadify.Authorization.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddPermissions(this IServiceCollection services)
    {
        services.AddAuthorizationCore();
        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        return services;
    }
}
