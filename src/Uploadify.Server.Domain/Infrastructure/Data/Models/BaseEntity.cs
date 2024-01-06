using Uploadify.Server.Domain.Infrastructure.Data.Contracts;

namespace Uploadify.Server.Domain.Infrastructure.Data.Models;

public class BaseEntity : IBaseEntity
{
    public bool IsActive { get; set; }
}
