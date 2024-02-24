namespace Uploadify.Server.Domain.Pagination.Models;

public class FolderQueryString : BaseQueryString
{
    public int? ParentFolderId { get; set; }
}
