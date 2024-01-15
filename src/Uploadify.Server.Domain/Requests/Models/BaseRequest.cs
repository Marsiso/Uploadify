using MediatR;

namespace Uploadify.Server.Domain.Requests.Models;

public class BaseRequest<TResponse> : IRequest<TResponse> where TResponse : BaseResponse;
