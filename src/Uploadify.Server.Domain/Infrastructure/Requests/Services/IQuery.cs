using MediatR;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;

namespace Uploadify.Server.Domain.Infrastructure.Requests.Services;

public interface IQuery<out TResponse> : IRequest<TResponse> where TResponse : BaseResponse;
