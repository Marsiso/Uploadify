namespace Uploadify.Server.Application.Files.Models;

public class FolderOverview
{
    public int FolderId { get; set; }
    public int? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TotalSize { get; set; }
    public int TotalCount { get; set; }
    public DateTime DateCreated { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime DateUpdated { get; set; }
    public string? UpdatedBy { get; set; }
}
