﻿using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;
using Uploadify.Server.Domain.Requests.Exceptions;
using Uploadify.Server.Domain.Requests.Services;

namespace Uploadify.Server.Application.Requests.Services;

public class AuthorizationPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationPipelineBehaviour(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ClaimsPrincipal? principal = _httpContextAccessor.HttpContext?.User;

        if (request is IRequestWithEmail requestWithEmail)
        {
            requestWithEmail.Email = principal?.FindFirst(OpenIddictConstants.Claims.Email)?.Value ?? throw new UnauthorizedException(string.Empty, string.Empty, string.Empty);
        }

        if (request is IRequestWithUserName requestWithUserName)
        {
            requestWithUserName.UserName = principal?.FindFirst(OpenIddictConstants.Claims.Name)?.Value ?? throw new UnauthorizedException(string.Empty, string.Empty, string.Empty);
        }

        return await next();
    }
}
