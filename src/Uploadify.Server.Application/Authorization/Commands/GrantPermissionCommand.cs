using System.Text.Json.Serialization;
using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Uploadify.Authorization.Models;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Localization.Constants;
using Uploadify.Server.Domain.Requests.Exceptions;
using Uploadify.Server.Domain.Requests.Models;
using Uploadify.Server.Domain.Requests.Services;
using static Uploadify.Server.Domain.Requests.Models.Status;

namespace Uploadify.Server.Application.Authorization.Commands;

public class GrantPermissionCommand : BaseRequest<GrantPermissionCommandResponse>, ICommand<GrantPermissionCommandResponse>, IRequestWithUserName
{
    [JsonIgnore]
    public string? UserName { get; set; }

    public string? Name { get; set; }
    public Permission? Permission { get; set; }
}

public class GrantPermissionCommandHandler : ICommandHandler<GrantPermissionCommand, GrantPermissionCommandResponse>
{
    private readonly RoleManager<Role> _manager;

    public GrantPermissionCommandHandler(RoleManager<Role> manager)
    {
        _manager = manager;
    }

    public async Task<GrantPermissionCommandResponse> Handle(GrantPermissionCommand request, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(request.Permission);
        Guard.IsNotNullOrWhiteSpace(request.Name);
        Guard.IsNotNullOrWhiteSpace(request.UserName);

        var role = await _manager.FindByNameAsync(request.Name);
        if (role == null)
        {
            return new GrantPermissionCommandResponse(NotFound, new RequestFailure
            {
                Exception = new EntityNotFoundException(request.Name, nameof(Role)),
                UserFriendlyMessage = Translations.RequestStatuses.NotFound
            });
        }

        role.Permission |= request.Permission.Value;

        var result = await _manager.UpdateAsync(role);
        if (result.Succeeded)
        {
            return new GrantPermissionCommandResponse();
        }

        return new GrantPermissionCommandResponse(InternalServerError, new RequestFailure
        {
            Exception = new InternalServerException(),
            UserFriendlyMessage = Translations.RequestStatuses.InternalServerError
        });
    }
}

public class GrantPermissionCommandResponse : BaseResponse
{
    public GrantPermissionCommandResponse()
    {
        Status = Ok;
    }

    public GrantPermissionCommandResponse(Status status, RequestFailure? failure) : base(status, failure)
    {
    }
}
