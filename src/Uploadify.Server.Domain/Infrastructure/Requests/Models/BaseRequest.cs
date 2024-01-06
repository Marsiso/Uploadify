using MediatR;

namespace Uploadify.Server.Domain.Infrastructure.Requests.Models;

public class BaseRequest<TResponse> : IRequest<TResponse> where TResponse : BaseResponse;
