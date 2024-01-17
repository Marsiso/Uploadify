using Uploadify.Server.Application.Authentication.DataTransferObjects;

namespace Uploadify.Server.Application.Authentication.ViewModels;

public class RegisterViewModel
{
    public string? ReturnUrl { get; set; }
    public RegisterForm Form { get; set; } = new RegisterForm();
    public Dictionary<string, string[]>? Errors { get; set; }
}
