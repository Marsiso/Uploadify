using MediatR;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;

namespace Uploadify.Server.Domain.Infrastructure.Requests.Contracts;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : BaseResponse;
