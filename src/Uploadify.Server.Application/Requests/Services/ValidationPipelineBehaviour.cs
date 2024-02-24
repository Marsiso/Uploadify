using FluentValidation;
using MediatR;
using Uploadify.Server.Application.Validations.Extensions;
using Uploadify.Server.Domain.Localization.Constants;
using Uploadify.Server.Domain.Requests.Exceptions;
using Uploadify.Server.Domain.Requests.Models;
using Uploadify.Server.Domain.Requests.Services;
using static Uploadify.Server.Domain.Requests.Models.Status;

namespace Uploadify.Server.Application.Requests.Services;

public class ValidationPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : BaseRequest<TResponse>, ICommand<TResponse>
    where TResponse : BaseResponse
{
    private readonly IValidator<TRequest>? _validator;

    public ValidationPipelineBehaviour(IValidator<TRequest>? validator)
    {
        _validator = validator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validator == null)
        {
            return await next();
        }

        var errors = (await _validator.ValidateAsync(new ValidationContext<TRequest>(request), cancellationToken)).DistinctErrorsByProperty();
        if (errors.Count <= 0)
        {
            return await next();
        }

        var response = Activator.CreateInstance<TResponse>();

        response.Status = BadRequest;
        response.Failure = new()
        {
            Errors = errors,
            UserFriendlyMessage = Translations.RequestStatuses.BadRequest,
            Exception = new BadRequestException(request.GetType().Name)
        };

        return response;
    }
}
