using MediatR;
using Microsoft.Extensions.Logging;
using Uploadify.Server.Domain.Localization.Constants;
using Uploadify.Server.Domain.Requests.Exceptions;
using Uploadify.Server.Domain.Requests.Models;
using static System.String;

namespace Uploadify.Server.Application.Requests.Services;

public class GlobalExceptionPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : BaseRequest<TResponse>
    where TResponse : BaseResponse
{
    private readonly ILogger<TRequest> _logger;

    public GlobalExceptionPipelineBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            var result = await next();

            result.Status = Status.InternalServerError;
            result.Failure = new() { UserFriendlyMessage = Translations.RequestStatuses.InternalServerError };

            return result;
        }
        catch (Exception exception)
        {
            var result = Activator.CreateInstance<TResponse>();

            switch (exception)
            {
                case EntityNotFoundException entityNotFoundException:
                    result.Status = Status.NotFound;
                    result.Failure = new() { UserFriendlyMessage = Translations.RequestStatuses.NotFound, Exception = entityNotFoundException };

                    _logger.LogInformation(Format("Service: '{0}' Entity: '{1}' Entity ID: '{2}' Message: 'Bad request.' Exception: '{3}'.",
                        typeof(TRequest).Name,
                        entityNotFoundException.EntityName,
                        entityNotFoundException.EntityID,
                        entityNotFoundException.Message));

                    return result;

                case BadRequestException badRequestException:
                    result.Status = Status.BadRequest;
                    result.Failure = new() { UserFriendlyMessage = Translations.RequestStatuses.BadRequest, Exception = badRequestException };

                    _logger.LogInformation($"Service: '{typeof(TRequest).Name}' Model: '{badRequestException.ObjectName}' Message: 'Bad request.' Exception: '{badRequestException.Message}'.");

                    return result;

                case UnauthorizedException unauthorizedException:
                    result.Status = Status.Unauthorized;
                    result.Failure = new() { UserFriendlyMessage = Translations.RequestStatuses.Unauthorized, Exception = unauthorizedException };

                    _logger.LogInformation(Format("Service: '{0}' UserName: '{1}' Resource: '{2}' Resource ID: '{3}' Message: 'Unauthorized access to resources.' Exception: '{4}'.",
                        typeof(TRequest).Name,
                        unauthorizedException.UserName,
                        unauthorizedException.ResourceName,
                        unauthorizedException.ResourceID,
                        unauthorizedException.Message));

                    return result;

                case InternalServerException internalServerException:
                    result.Status = Status.InternalServerError;
                    result.Failure = new() { UserFriendlyMessage = Translations.RequestStatuses.Unauthorized, Exception = internalServerException };

                    _logger.LogInformation($"Service: '{typeof(TRequest).Name}' Message: 'Internal server error.' Exception: '{internalServerException.Message}'.");

                    return result;

                case ValidationException validationException:
                    result.Status = Status.BadRequest;
                    result.Failure = new() { UserFriendlyMessage = Translations.RequestStatuses.Unauthorized, Exception = validationException };

                    _logger.LogInformation($"Service: '{typeof(TRequest).Name}' Model: '{validationException.ObjectName}' Message: 'Model validation failure.' Exception: '{validationException.Message}'.");

                    return result;

                case OperationCanceledException operationCanceledException:
                    result.Status = Status.ClientClosedRequest;
                    result.Failure = new() { UserFriendlyMessage = Translations.RequestStatuses.ClientCancelledOperation, Exception = operationCanceledException };

                    _logger.LogInformation($"Service: '{typeof(TRequest).Name}' Message: 'Client closed request.' Exception: '{operationCanceledException.Message}'.");

                    return result;

                default:
                    result.Status = Status.InternalServerError;
                    result.Failure = new() { UserFriendlyMessage = Translations.RequestStatuses.InternalServerError, Exception = exception };

                    _logger.LogInformation($"Service: '{typeof(TRequest).Name}' Message: 'Internal server error.' Exception: '{exception.Message}'.");

                    return result;
            }
        }
    }
}
