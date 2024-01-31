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
            .MustAsync((command, _, cancellationToken) => IsAuthorized(command.UserName, command.Permission, cancellationToken))
            .WithMessage(Translations.Validations.Unauthorized);
    }

    private async Task<bool> IsAuthorized(string? userName, Permission? permission, CancellationToken cancellationToken)
    {
        if (!permission.HasValue || permission.Value.HasFlag(Permission.All))
        {
            return false;
        }

        var response = await _mediator.Send(new UserQuery(userName), cancellationToken);
        if (response is not { Status: Status.Ok, User: not null })
        {
            return false;
        }

        if (permission is Permission.ViewRoles or Permission.EditRoles or Permission.ViewPermissions or Permission.EditPermissions)
        {
            return response.User.Roles?.Any(role => role.Role != null && role.Role.Permission.HasFlag(Permission.All)) ?? false;
        }

        return response.User.Roles?.Any(role => role.Role != null && (role.Role.Permission.HasFlag(Permission.EditPermissions) || role.Role.Permission.HasFlag(Permission.All))) ?? false;
    }
}
