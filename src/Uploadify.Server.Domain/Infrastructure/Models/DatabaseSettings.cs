namespace Uploadify.Server.Domain.Infrastructure.Models;

public class DatabaseSettings
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Database { get; set; }
    public required string Host { get; set; }
    public required int Port { get; set; }
    public required bool Pooling { get; set; }
    public required bool IsSeedEnabled { get; set; }
}
