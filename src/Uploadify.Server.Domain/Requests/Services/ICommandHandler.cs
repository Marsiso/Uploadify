using MediatR;
using Uploadify.Server.Domain.Requests.Models;

namespace Uploadify.Server.Domain.Requests.Services;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : BaseResponse;
