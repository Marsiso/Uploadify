using FluentValidation;
using MediatR;
using Uploadify.Authorization.Models;
using Uploadify.Server.Application.Auth.Commands;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;

namespace Uploadify.Server.Application.Auth.Validators;

public class RevokePermissionCommandValidator : AbstractValidator<RevokePermissionCommand>
{
    private readonly IMediator _mediator;

    public RevokePermissionCommandValidator(IMediator mediator)
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
            .MustAsync((command, _, cancellationToken) => IsAuthorized(command.Name, command.UserName, command.Permission, cancellationToken))
            .WithMessage(Translations.Validations.Unauthorized);
    }

    private async Task<bool> IsAuthorized(string?  roleName, string? userName, Permission? permission, CancellationToken cancellationToken)
    {
        if (!permission.HasValue || permission.Value.HasFlag(Permission.All))
        {
            return false;
        }

        var roleResponse = await _mediator.Send(new GetRoleQuery(roleName), cancellationToken);
        if (roleResponse is not { Status: Status.Ok, Role: not null } || roleResponse.Role.Permission.HasFlag(Permission.All))
        {
            return false;
        }

        var userResponse = await _mediator.Send(new GetUserQuery(userName), cancellationToken);
        if (userResponse is not { Status: Status.Ok, User: not null })
        {
            return false;
        }

        if (permission is Permission.ViewRoles or Permission.EditRoles or Permission.ViewPermissions or Permission.EditPermissions)
        {
            return userResponse.User.Roles?.Any(role => role.Role != null && role.Role.Permission.HasFlag(Permission.All)) ?? false;
        }

        return userResponse.User.Roles?.Any(role => role.Role != null && (role.Role.Permission.HasFlag(Permission.EditPermissions) || role.Role.Permission.HasFlag(Permission.All))) ?? false;
    }
}
