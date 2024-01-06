namespace Uploadify.Server.Domain.Infrastructure.Requests.Models;

public enum Status
{
    Ok = 200,
    Created = 201,
    Accepted = 202,
    NoContent = 204,
    MovedPermanently = 301,
    Found = 302,
    NotModified = 304,
    TemporaryRedirect = 307,
    PermanentRedirect = 308,
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    MethodNotAllowed = 405,
    Conflict = 409,
    PayloadTooLarge = 413,
    TooEarly = 425,
    TooManyRequest = 429,
    NoResponse = 444,
    ClientClosedRequest = 499,
    InternalServerError = 500,
}
