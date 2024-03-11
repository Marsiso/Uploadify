using Uploadify.Server.Domain.Data.Models;

namespace Uploadify.Server.Domain.Application.Models;

public class CodeListItem : BaseEntity
{
    public int Id { get; set; }
    public int CodeListId { get; set; }
    public string Value { get; set; } = string.Empty;
}
