using MediatR;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;

namespace Uploadify.Server.Domain.Infrastructure.Requests.Contracts;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : BaseResponse;
