using System.Text.Json.Serialization;

namespace Uploadify.Server.Domain.Infrastructure.Requests.Models;

public class RequestFailure
{
    public Dictionary<string, string[]>? Errors { get; set; }
    public string? UserFriendlyMessage { get; set; }

    [JsonIgnore] public Exception? Exception { get; set; }
}
