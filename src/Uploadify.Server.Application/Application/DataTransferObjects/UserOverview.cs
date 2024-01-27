namespace Uploadify.Server.Application.Application.DataTransferObjects;

public class UserOverview
{
    public string UserName { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Picture { get; set; }
}
