namespace Uploadify.Server.Application.Files.Models;

public class FolderSummary : FolderOverview
{
    public bool HasFiles => Files != null && Files.Any();
    public bool HasFolders => Folders != null && Folders.Any();
    public bool HasLinks => Links != null && Links.Any();

    public FolderOverview? Parent { get; set; }
    public IEnumerable<FolderLink>? Links { get; set; }
    public IEnumerable<FolderOverview>? Folders { get; set; }
    public IEnumerable<FileOverview>? Files { get; set; }
}
