using System.Diagnostics.CodeAnalysis;
using Uploadify.Client.Integration.Resources;

namespace Uploadify.Client.Application.Files.Models;

public record DashboardItem
{
    public DashboardItem(string name, bool isFolder, bool isParent, FileOverview? file, FolderOverview? folder)
    {
        Name = name;
        IsFolder = isFolder;
        IsParent = isParent;
        File = file;
        Folder = folder;
    }

    public DashboardItem(FolderOverview folder, bool isParent = false)
    {
        Name = folder.Name;
        IsFolder = true;
        IsParent = isParent;
        Folder = folder;
    }

    public DashboardItem(FileOverview file)
    {
        Name = file.Name;
        File = file;
    }

    public string Name { get; init; }

    [MemberNotNullWhen(true, nameof(Folder))]
    [MemberNotNullWhen(false, nameof(File))]
    public bool IsFolder { get; init; }

    public bool IsParent { get; init; }

    public FileOverview? File { get; init; }
    public FolderOverview? Folder { get; init; }

}
