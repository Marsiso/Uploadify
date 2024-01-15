using MediatR;
using Uploadify.Server.Domain.Requests.Models;

namespace Uploadify.Server.Domain.Requests.Services;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : BaseResponse;
