namespace Uploadify.Server.Application.Files.Models;

public class FileMetadata
{
    public string Location { get; set; } = string.Empty;
    public string UnsafeName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long Size { get; set; }
}
