using Uploadify.Server.Domain.Application.Models;

namespace Uploadify.Server.Domain.Common.Contracts;

public interface IChangeTrackingBaseEntity : IBaseEntity
{
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }

    public User? UserCreatedBy { get; set; }
    public User? UserUpdatedBy { get; set; }
}
