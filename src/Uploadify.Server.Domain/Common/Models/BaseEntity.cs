using Uploadify.Server.Domain.Common.Contracts;

namespace Uploadify.Server.Domain.Common.Models;

public class BaseEntity : IBaseEntity
{
    public bool IsActive { get; set; }
}
