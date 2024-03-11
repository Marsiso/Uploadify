using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;
using Uploadify.Server.Domain.Infrastructure.Requests.Contracts;
using Uploadify.Server.Domain.Infrastructure.Requests.Exceptions;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using static System.String;
using static Uploadify.Server.Domain.Infrastructure.Requests.Models.Status;

namespace Uploadify.Server.Application.Infrastructure.Requests.Services;

public class AuthorizationPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull where TResponse : BaseResponse
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
            requestWithEmail.Email = principal?.FindFirst(OpenIddictConstants.Claims.Email)?.Value ?? throw new UnauthorizedException(Empty, Empty, Empty);
            if (IsNullOrWhiteSpace(requestWithEmail.Email))
            {
                var response = Activator.CreateInstance<TResponse>();

                response.Status = Unauthorized;
                response.Failure = new()
                {
                    UserFriendlyMessage = Translations.RequestStatuses.Unauthorized,
                    Exception = new UnauthorizedException(Empty, Empty, Empty)
                };

                return response;
            }
        }

        if (request is IRequestWithUserName requestWithUserName)
        {
            requestWithUserName.UserName = principal?.FindFirst(OpenIddictConstants.Claims.Name)?.Value ?? throw new UnauthorizedException(Empty, Empty, Empty);
            if (IsNullOrWhiteSpace(requestWithUserName.UserName))
            {
                var response = Activator.CreateInstance<TResponse>();

                response.Status = Unauthorized;
                response.Failure = new()
                {
                    UserFriendlyMessage = Translations.RequestStatuses.Unauthorized,
                    Exception = new UnauthorizedException(Empty, Empty, Empty)
                };

                return response;
            }
        }

        return await next();
    }
}
