using Uploadify.Server.Application.Authentication.DataTransferObjects;

namespace Uploadify.Server.Application.Authentication.ViewModels;

public class LoginViewModel
{
    public string? ReturnUrl { get; set; }
    public LoginForm Form { get; set; } = new LoginForm();
    public Dictionary<string, string[]>? Errors { get; set; }
}
