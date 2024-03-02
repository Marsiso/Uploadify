using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Data.Models;

namespace Uploadify.Server.Domain.Files.Models;

public class File : ChangeTrackingBaseEntity
{
    public int Id { get; set; }
    public int FolderId { get; set; }
    public int? CategoryId { get; set; }
    public string SafeName { get; set; } = string.Empty;
    public string UnsafeName { get; set; } = string.Empty;
    public long Size { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;

    public Folder? Folder { get; set; }
    public CodeListItem? Category { get; set; }
    public ICollection<SharedFile>? Users { get; set; }
}
