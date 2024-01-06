using Uploadify.Server.Domain.Infrastructure.Data.Models;

namespace Uploadify.Server.Domain.Application.Models;

public class CodeList : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<CodeListItem>? CodeListItems { get; set; }
}
