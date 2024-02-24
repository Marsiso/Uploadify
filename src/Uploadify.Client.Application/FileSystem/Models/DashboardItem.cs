namespace Uploadify.Client.Application.FileSystem.Models;

public record DashboardItem(string Name, DateTime DateCreated, DateTime DateUpdated, bool IsFolder, bool IsRootFolder);
