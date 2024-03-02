using Uploadify.Server.Application.Auth.Models;

namespace Uploadify.Server.Application.Auth.ViewModels;

public class RegisterViewModel
{
    public string? ReturnUrl { get; set; }
    public RegisterForm Form { get; set; } = new();
    public Dictionary<string, string[]>? Errors { get; set; }
}
