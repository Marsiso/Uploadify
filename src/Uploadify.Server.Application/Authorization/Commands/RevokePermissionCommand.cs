﻿using System.Text.Json.Serialization;
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

public class RevokePermissionCommand : BaseRequest<RevokePermissionCommandResponse>, ICommand<RevokePermissionCommandResponse>, IRequestWithUserName
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
            return new RevokePermissionCommandResponse(NotFound, new RequestFailure
            {
                Exception = new EntityNotFoundException(request.Name, nameof(Role)),
                UserFriendlyMessage = Translations.RequestStatuses.NotFound
            });
        }

        role.Permission &= ~request.Permission.Value;

        var result = await _manager.UpdateAsync(role);
        if (result.Succeeded)
        {
            return new RevokePermissionCommandResponse();
        }

        return new RevokePermissionCommandResponse(InternalServerError, new RequestFailure
        {
            Exception = new InternalServerException(),
            UserFriendlyMessage = Translations.RequestStatuses.InternalServerError
        });
    }
}

public class RevokePermissionCommandResponse : BaseResponse
{
    public RevokePermissionCommandResponse()
    {
        Status = Ok;
    }

    public RevokePermissionCommandResponse(Status status, RequestFailure? failure) : base(status, failure)
    {
    }
}