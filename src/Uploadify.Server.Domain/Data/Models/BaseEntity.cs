using Uploadify.Server.Domain.Data.Contracts;

namespace Uploadify.Server.Domain.Data.Models;

public class BaseEntity : IBaseEntity
{
    public bool IsActive { get; set; }
}
