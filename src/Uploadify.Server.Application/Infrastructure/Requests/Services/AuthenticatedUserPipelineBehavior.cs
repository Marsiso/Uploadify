﻿using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using Uploadify.Server.Domain.Infrastructure.Requests.Services;

namespace Uploadify.Server.Application.Infrastructure.Requests.Services;

public class AuthenticationPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : BaseRequest<TResponse>
    where TResponse : BaseResponse
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationPipelineBehaviour(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ClaimsPrincipal? principal = _httpContextAccessor.HttpContext?.User;

        if (request is IRequestWithEmail requestWithEmail)
        {
            requestWithEmail.Email = principal?.FindFirst(OpenIddictConstants.Claims.Email)?.Value;
        }

        if (request is IRequestWithLogin requestWithLogin)
        {
            requestWithLogin.Login = principal?.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        }

        return await next();
    }
}