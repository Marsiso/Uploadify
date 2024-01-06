using FluentValidation;
using MediatR;
using Uploadify.Server.Application.Infrastructure.Validators.Extensions;
using Uploadify.Server.Domain.Infrastructure.Localization;
using Uploadify.Server.Domain.Infrastructure.Requests.Exceptions;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using Uploadify.Server.Domain.Infrastructure.Requests.Services;

namespace Uploadify.Server.Application.Infrastructure.Requests.Services;

public class ValidationPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : BaseRequest<TResponse>, ICommand<TResponse>
    where TResponse : BaseResponse
{
    private readonly IValidator<TRequest> _validator;

    public ValidationPipelineBehaviour(IValidator<TRequest> validator)
    {
        _validator = validator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var errors = (await _validator.ValidateAsync(new ValidationContext<TRequest>(request), cancellationToken)).DistinctErrorsByProperty();
        if (errors.Count > 0)
        {
            var response = Activator.CreateInstance<TResponse>();

            response.Status = Status.BadRequest;
            response.Failure = new RequestFailure
            {
                Errors = errors,
                UserFriendlyMessage = Translations.RequestStatuses.BadRequest,
                Exception = new BadRequestException(request.GetType().Name)
            };

            return response;
        }


        return await next();
    }
}
