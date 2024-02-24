using Uploadify.Server.Application.Application.DataTransferObjects;

namespace Uploadify.Server.Application.FileSystem.DataTransferObjects;

public class FileDetail
{
    public int Id { get; set; }
    public int FolderId { get; set; }
    public int? CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public long Size { get; set; }
    public string Extension { get; set; } = string.Empty;
    public FolderOverview Folder { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
    public UserOverview? UserCreatedBy { get; set; }
    public UserOverview? UserUpdatedBy { get; set; }
}
