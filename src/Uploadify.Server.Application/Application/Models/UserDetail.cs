namespace Uploadify.Server.Application.Application.Models;

public class UserDetail
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public bool PhoneNumberConfirmed { get; set; }
    public string? Picture { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
