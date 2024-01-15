using Microsoft.AspNetCore.Identity;
using Uploadify.Server.Domain.Common.Contracts;

namespace Uploadify.Server.Domain.Application.Models;

public class UserClaim : IdentityUserClaim<string>, IBaseEntity
{
    public bool IsActive { get; set; }
}
