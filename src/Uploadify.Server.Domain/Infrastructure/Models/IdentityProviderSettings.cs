namespace Uploadify.Server.Domain.Infrastructure.Models;

public class IdentityProviderSettings
{
    public required string Issuer { get; set; }
    public required string[] SupportedClaims { get; set; }
    public required string[] SupportedScopes { get; set; }
}
