namespace Uploadify.Server.Domain.Infrastructure.Models;

public class SystemSettings
{
    public const string SectionName = "Application";

    public DatabaseSettings? Database { get; set; }
    public ClientSettings? Client { get; set; }
    public ClientSettings? Api { get; set; }
    public IdentityProviderSettings? IdentityProvider { get; set; }
    public ReverseProxySettings? ReverseProxy { get; set; }
    public HangfireSettings? Hangfire { get; set; }
}

public class ReverseProxySettings
{
    public const string SectionName = "ReverseProxy";

    public string? Uri { get; set; }
}

public class IdentityProviderSettings
{
    public string? Issuer { get; set; }
    public string[]? SupportedClaims { get; set; }
    public string[]? SupportedScopes { get; set; }
}

public class DatabaseSettings
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Database { get; set; }
    public string? Host { get; set; }
    public int Port { get; set; }
    public bool Pooling { get; set; }
    public bool IsSeedEnabled { get; set; }
}

public class ClientSettings
{
    public string? ID { get; set; }
    public string? Secret { get; set; }
    public string? DisplayName { get; set; }
    public string? RedirectUri { get; set; }
    public string? PostLogoutRedirectUri { get; set; }
}

public class HangfireSettings
{
    public string? Login { get; set; }
    public string? Password { get; set; }
}
