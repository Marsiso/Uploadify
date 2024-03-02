namespace Uploadify.Server.Domain.Infrastructure.Requests.Contracts;

public interface IRequestWithEmail
{
    public string? Email { get; set; }
}
