using Uploadify.Client.Application.Files.Models;
using Uploadify.Client.Domain.Resources.Models;
using Uploadify.Client.Integration.Resources;

namespace Uploadify.Client.Application.Files.Helpers;

public static class FolderHelpers
{
    public static List<DashboardItem> GetDashboardItems(ResourceResponse<FolderSummary>? response)
    {
        var dashboard = new List<DashboardItem>();
        if (response is not { IsSuccess: true })
        {
            return dashboard;
        }

        dashboard.AddRange(response.Resource.Folders.Select(folder => new DashboardItem(folder.Name, true, false, null, folder)));
        dashboard.AddRange(response.Resource.Files.Select(file => new DashboardItem(file.Name, false, false, file, null)));

        return dashboard;
    }
}
