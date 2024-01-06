using static System.String;

namespace Uploadify.Server.Application.Authentication.ViewModels;

public class ErrorViewModel
{
    public string? RequestID { get; set; }
    public bool ShowRequestID => !IsNullOrEmpty(RequestID);
}
