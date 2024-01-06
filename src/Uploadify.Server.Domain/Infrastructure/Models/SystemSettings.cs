namespace Uploadify.Server.Domain.Infrastructure.Models;

public class SystemSettings
{
    public const string SectionName = "Application";

    public required DatabaseSettings Database { get; set; }
    public required ClientSettings Client { get; set; }
    public required IdentityProviderSettings IdentityProvider { get; set; }
}
