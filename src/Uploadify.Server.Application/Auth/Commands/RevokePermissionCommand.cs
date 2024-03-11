using System.Text.Json.Serialization;
using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Uploadify.Authorization.Models;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;
using Uploadify.Server.Domain.Infrastructure.Requests.Contracts;
using Uploadify.Server.Domain.Infrastructure.Requests.Exceptions;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using static Uploadify.Server.Domain.Infrastructure.Requests.Models.Status;

namespace Uploadify.Server.Application.Auth.Commands;

public class RevokePermissionCommand : IBaseRequest<RevokePermissionCommandResponse>, ICommand<RevokePermissionCommandResponse>, IRequestWithUserName
{
    [JsonIgnore]
    public string? UserName { get; set; }

    public string? Name { get; set; }
    public Permission? Permission { get; set; }
}

public class RevokePermissionCommandHandler : ICommandHandler<RevokePermissionCommand, RevokePermissionCommandResponse>
{
    private readonly RoleManager<Role> _manager;

    public RevokePermissionCommandHandler(RoleManager<Role> manager)
    {
        _manager = manager;
    }

    public async Task<RevokePermissionCommandResponse> Handle(RevokePermissionCommand request, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(request.Permission);
        Guard.IsNotNullOrWhiteSpace(request.Name);
        Guard.IsNotNullOrWhiteSpace(request.UserName);

        var role = await _manager.FindByNameAsync(request.Name);
        if (role == null)
        {
            return new(NotFound, new()
            {
                Exception = new EntityNotFoundException(request.Name, nameof(Role)),
                UserFriendlyMessage = Translations.RequestStatuses.NotFound
            });
        }

        role.Permission &= ~request.Permission.Value;

        var result = await _manager.UpdateAsync(role);
        if (result.Succeeded)
        {
            return new();
        }

        return new(InternalServerError, new()
        {
            Exception = new InternalServerException(),
            UserFriendlyMessage = Translations.RequestStatuses.InternalServerError
        });
    }
}

public class RevokePermissionCommandResponse : BaseResponse
{
    public RevokePermissionCommandResponse() : base(Ok)
    {
    }

    public RevokePermissionCommandResponse(Status status, RequestFailure? failure) : base(status, failure)
    {
    }
}
