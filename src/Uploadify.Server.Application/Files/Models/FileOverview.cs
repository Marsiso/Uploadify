namespace Uploadify.Server.Application.Files.Models;

public class FileOverview
{
    public int FileId { get; set; }
    public int FolderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public long Size { get; set; }
    public bool IsPublic { get; set; }
    public string Extension { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
}
