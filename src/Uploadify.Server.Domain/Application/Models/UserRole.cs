using Microsoft.AspNetCore.Identity;
using Uploadify.Server.Domain.Data.Contracts;

namespace Uploadify.Server.Domain.Application.Models;

public class UserRole : IdentityUserRole<string>, IChangeTrackingBaseEntity
{
    public bool IsActive { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }

    public User? User { get; set; }
    public Role? Role { get; set; }
    public User? UserCreatedBy { get; set; }
    public User? UserUpdatedBy { get; set; }
}
