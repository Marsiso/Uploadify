namespace Uploadify.Server.Domain.Infrastructure.Requests.Contracts;

public interface IRequestWithUserName
{
    public string? UserName { get; set; }
}
