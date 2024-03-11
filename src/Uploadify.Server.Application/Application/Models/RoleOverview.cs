using Uploadify.Authorization.Models;

namespace Uploadify.Server.Application.Application.Models;

public class RoleOverview
{
    public string Name { get; set; } = string.Empty;
    public Permission Permission { get; set; }
    public UserOverview? UserCreatedBy { get; set; }
    public DateTime DateCreated { get; set; }
    public UserOverview? UserUpdatedBy { get; set; }
    public DateTime DateUpdated { get; set; }
}
