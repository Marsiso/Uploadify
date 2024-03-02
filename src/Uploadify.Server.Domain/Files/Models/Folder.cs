using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Data.Models;

namespace Uploadify.Server.Domain.Files.Models;

public class Folder : ChangeTrackingBaseEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int? CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TotalSize { get; set; }
    public int TotalCount { get; set; }

    public User? User { get; set; }
    public Folder? Parent { get; set; }
    public CodeListItem? Category { get; set; }
    public ICollection<File>? Files { get; set; }
    public ICollection<Folder>? Children { get; set; }
}
