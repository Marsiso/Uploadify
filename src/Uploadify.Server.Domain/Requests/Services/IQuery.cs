using MediatR;
using Uploadify.Server.Domain.Requests.Models;

namespace Uploadify.Server.Domain.Requests.Services;

public interface IQuery<out TResponse> : IRequest<TResponse> where TResponse : BaseResponse;
