using MudBlazor;

namespace Uploadify.Client.Application.Files.Models;

public class DashboardBreadcrumbItem : BreadcrumbItem
{
    public int? FolderID { get; set; }

    public DashboardBreadcrumbItem(string text, string? href, bool disabled = false, string? icon = null, int? folderID = null) : base(text, href, disabled, icon)
    {
        FolderID = folderID;
    }
}
