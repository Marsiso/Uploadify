namespace Uploadify.Server.IdentityServer.Views.Shared;

public class InputViewModel
{
    public string PropertyName { get; set; } = string.Empty;
    public string InputType { get; set; } = string.Empty;
    public object? Value { get; set; }
    public string Label { get; set; } = string.Empty;
    public string[]? Errors { get; set; }
    public bool IsOptional { get; set; }
}
