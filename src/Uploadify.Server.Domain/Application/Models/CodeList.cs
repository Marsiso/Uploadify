using Uploadify.Server.Domain.Common.Models;

namespace Uploadify.Server.Domain.Application.Models;

public class CodeList : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<CodeListItem>? CodeListItems { get; set; }
}
