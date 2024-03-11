using Uploadify.Server.Application.Auth.Models;

namespace Uploadify.Server.Application.Auth.ViewModels;

public class LoginViewModel
{
    public string? ReturnUrl { get; set; }
    public LoginForm Form { get; set; } = new();
    public Dictionary<string, string[]>? Errors { get; set; }
}
