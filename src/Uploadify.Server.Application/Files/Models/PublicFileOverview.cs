using Uploadify.Server.Application.Application.Models;

namespace Uploadify.Server.Application.Files.Models;

public class PublicFileOverview
{
    public int FileId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public string OwnerUserName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long Size { get; set; }
}
