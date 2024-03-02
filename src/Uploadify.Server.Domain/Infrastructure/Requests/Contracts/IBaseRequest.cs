using MediatR;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;

namespace Uploadify.Server.Domain.Infrastructure.Requests.Contracts;

public interface IBaseRequest<out TResponse> : IRequest<TResponse> where TResponse : BaseResponse;
