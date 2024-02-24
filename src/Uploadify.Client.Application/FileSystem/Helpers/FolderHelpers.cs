using Uploadify.Client.Application.FileSystem.Models;
using Uploadify.Client.Domain.Resources.Models;
using Uploadify.Client.Integration.Resources;
using static System.String;

namespace Uploadify.Client.Application.FileSystem.Helpers;

public static class FolderHelpers
{
    public const int MaxDisplayedFileNameLength = 48;

    public static string GetShortFileName(DashboardItem item)
    {
        var filename = Path.GetFileNameWithoutExtension(item.Name);
        if (IsNullOrWhiteSpace(filename))
        {
            return item.Name;
        }

        if (filename.Length > MaxDisplayedFileNameLength)
        {
            return $"{filename[..MaxDisplayedFileNameLength]}...";
        }

        return filename;
    }

    public static List<DashboardItem> GetDashboardItems(ResourceResponse<FolderSummary> response)
    {
        var dashboard = new List<DashboardItem>();

        if (!response.IsSuccess)
        {
            return dashboard;
        }

        dashboard.Add(new(response.Resource.Name, response.Resource.DateCreated.Value, response.Resource.DateUpdated.Value, true, true));
        dashboard.AddRange(response.Resource.Folders.Select(folder => new DashboardItem(folder.Name, folder.DateCreated.Value, folder.DateUpdated.Value, true, false)));
        dashboard.AddRange(response.Resource.Files.Select(file => new DashboardItem(file.Name, file.DateCreated.Value, file.DateUpdated.Value, false, false)));

        return dashboard;
    }
}
