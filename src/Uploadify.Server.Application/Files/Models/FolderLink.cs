namespace Uploadify.Server.Application.Files.Models;

public class FolderLink
{
    public int FolderId { get; set; }
    public int? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
}
