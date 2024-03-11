using static System.String;

namespace Uploadify.Server.Application.Auth.ViewModels;

public class ErrorViewModel
{
    public string? RequestID { get; set; }
    public bool ShowRequestID => !IsNullOrEmpty(RequestID);
}
