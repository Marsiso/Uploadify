namespace Uploadify.Server.Domain.Infrastructure.Pagination.Models.Files;

public class FolderQueryString : BaseQueryString
{
    public int? ParentFolderId { get; set; }
}
