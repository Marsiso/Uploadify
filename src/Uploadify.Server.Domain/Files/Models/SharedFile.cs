using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Data.Models;

namespace Uploadify.Server.Domain.Files.Models;

public class SharedFile : ChangeTrackingBaseEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int FileId { get; set; }

    public User? User { get; set; }
    public File? File { get; set; }
}
