namespace Uploadify.Server.Application.FileSystem.DataTransferObjects;

public class FolderSummary : FolderOverview
{
    public bool HasFiles => Files != null && Files.Any();
    public bool HasFolders => Folders != null && Folders.Any();

    public IEnumerable<FolderOverview>? Folders { get; set; }
    public IEnumerable<FileOverview>? Files { get; set; }
}
