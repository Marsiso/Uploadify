namespace Uploadify.Client.Application.Utilities.Services;

public class MobileViewManager
{
    public bool IsDesktop { get; set; }
    public bool IsMobile() => !IsDesktop;
}
