namespace Uploadify.Server.Domain.Infrastructure.Models;

public class ClientSettings
{
    public required string ID { get; set; }
    public required string Secret { get; set; }
    public required string DisplayName { get; set; }
    public required string RedirectUri { get; set; }
    public required string PostLogoutRedirectUri { get; set; }
}
