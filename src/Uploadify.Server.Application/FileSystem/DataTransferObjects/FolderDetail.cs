using Uploadify.Server.Application.Application.DataTransferObjects;

namespace Uploadify.Server.Application.FileSystem.DataTransferObjects;

public class FolderDetail
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int? CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TotalSize { get; set; }
    public int TotalCount { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
    public UserOverview? UserCreatedBy { get; set; }
    public UserOverview? UserUpdatedBy { get; set; }
}
