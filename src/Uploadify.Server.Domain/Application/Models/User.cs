using Microsoft.AspNetCore.Identity;
using Uploadify.Server.Domain.Common.Contracts;
using Uploadify.Server.Domain.Files.Models;

namespace Uploadify.Server.Domain.Application.Models;

public class User : IdentityUser, IChangeTrackingBaseEntity
{
    public string GivenName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public string? Picture { get; set; }
    public DateTime? DateLastLoggedIn { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
    public bool IsActive { get; set; }

    public User? UserCreatedBy { get; set; }
    public User? UserUpdatedBy { get; set; }
    public ICollection<UserClaim>? Claims { get; set; }
    public ICollection<UserLogin>? Logins { get; set; }
    public ICollection<UserToken>? Tokens { get; set; }
    public ICollection<UserRole>? Roles { get; set; }
    public ICollection<Folder>? Folders { get; set; }
    public ICollection<FileLink>? FileLinks { get; set; }
}
