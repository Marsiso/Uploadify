using Microsoft.AspNetCore.Identity;
using Uploadify.Server.Domain.Authorization.Models;
using Uploadify.Server.Domain.Common.Contracts;

namespace Uploadify.Server.Domain.Application.Models;

public class Role : IdentityRole, IChangeTrackingBaseEntity
{
    public Permission Permission { get; set; } = Permission.None;
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
    public bool IsActive { get; set; }

    public User? UserCreatedBy { get; set; }
    public User? UserUpdatedBy { get; set; }
    public ICollection<RoleClaim>? Claims { get; set; }
    public ICollection<UserRole>? Users { get; set; }
}
