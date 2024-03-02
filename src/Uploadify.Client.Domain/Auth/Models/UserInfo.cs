namespace Uploadify.Client.Domain.Auth.Models;

public class UserInfo
{
    public static readonly UserInfo Anonymous = new();

    public string NameClaimType { get; set; } = string.Empty;
    public string RoleClaimType { get; set; } = string.Empty;
    public bool IsAuthenticated { get; set; }
    public ICollection<ClaimValue>? Claims { get; set; }
}
