using FluentValidation;
using MediatR;
using Uploadify.Authorization.Models;
using Uploadify.Server.Application.Authorization.Commands;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Domain.Localization.Constants;
using Uploadify.Server.Domain.Requests.Models;

namespace Uploadify.Server.Application.Authorization.Validators;

public class GrantPermissionCommandValidator : AbstractValidator<GrantPermissionCommand>
{
    private readonly IMediator _mediator;

    public GrantPermissionCommandValidator(IMediator mediator)
    {
        _mediator = mediator;

        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage(Translations.Validations.RoleNameRequired);

        RuleFor(command => command.Permission)
            .NotNull()
            .WithMessage(Translations.Validations.PermissionRequired)
            .IsInEnum()
            .WithMessage(Translations.Validations.PermissionIsNotInEnum)
            .Must(PermissionRequired)
            .WithMessage(Translations.Validations.PermissionRequired)
            .MustAsync((command, _, cancellationToken) => IsAuthorized(command.UserName, command.Name, command.Permission, cancellationToken))
            .WithMessage(Translations.Validations.Unauthorized);
    }

    private static bool PermissionRequired(Permission? permission)
    {
        return permission.HasValue && !permission.Value.HasFlag(Permission.None);
    }

    private async Task<bool> IsAuthorized(string? userName, string? roleName, Permission? permission, CancellationToken cancellationToken)
    {
        if (!permission.HasValue || permission.Value.HasFlag(Permission.All))
        {
            return false;
        }

        var roleResponse = await _mediator.Send(new GetRoleQuery(roleName), cancellationToken);
        if (roleResponse is not { Status: Status.Ok, Role: not null })
        {
            return false;
        }

        if (roleResponse.Role.Permission.HasFlag(Permission.All))
        {
            return false;
        }

        var userResponse = await _mediator.Send(new GetUserQuery(userName), cancellationToken);
        if (userResponse is not { Status: Status.Ok, User.Roles.Count: > 0 })
        {
            return false;
        }

        if (permission.Value.HasFlag(Permission.ViewRoles) ||
            permission.Value.HasFlag(Permission.EditRoles) ||
            permission.Value.HasFlag(Permission.ViewPermissions) ||
            permission.Value.HasFlag(Permission.EditPermissions))
        {
            return userResponse.User.Roles.Any(role => role.Role != null && role.Role.Permission.HasFlag(Permission.All));
        }

        return userResponse.User.Roles.Any(role => role.Role != null && role.Role.Permission.HasFlag(Permission.EditPermissions));
    }
}
