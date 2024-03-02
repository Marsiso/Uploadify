using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Uploadify.Authorization.Models;
using Uploadify.Client.Application.Resources.Services;
using Uploadify.Client.Core.Infrastructure.Services;
using Uploadify.Client.Domain.Localization.Constants;
using Uploadify.Client.Domain.Resources.Models;
using Uploadify.Client.Integration.Resources;
using static System.String;

namespace Uploadify.Client.Application.Application.Services;

public class PermissionService : BaseResourceService<PermissionService>
{
    public PermissionService(ApiCallWrapper apiCallWrapper, IStringLocalizer localizer, ILogger<PermissionService> logger) : base(apiCallWrapper, localizer, logger)
    {
    }

    public async Task<ResourceResponse> AssignPermission(Permission? permission, RoleOverview? role, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!permission.HasValue)
            {
                return new([Localizer[Translations.Validations.PermissionRequired]]);
            }

            if (IsNullOrWhiteSpace(role?.Name))
            {
                return new([Localizer[Translations.Validations.RoleNameRequired]]);
            }

            var response = await ApiCallWrapper.Call(client => client.ApiPermissionGrantAsync(new() { Name = role.Name, Permission = permission }, cancellationToken));
            if (response is not { Status: Status.Ok })
            {
                return new(HandleServerErrorMessages(response?.Failure));
            }

            return new();
        }
        catch (Exception exception)
        {
            LogError(exception);
        }

        return new([Localizer[Translations.RequestStatuses.InternalServerError]]);
    }

    public async Task<ResourceResponse> RevokePermission(Permission? permission, RoleOverview? role, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await ApiCallWrapper.Call(client => client.ApiPermissionRevokeAsync(new()
            {
                Name = role.Name,
                Permission = permission
            }, cancellationToken));

            if (response is not { Status: Status.Ok })
            {
                return new(HandleServerErrorMessages(response?.Failure));
            }

            return new();
        }
        catch (Exception exception)
        {
            LogError(exception);
        }

        return new([Localizer[Translations.RequestStatuses.InternalServerError]]);
    }
}
