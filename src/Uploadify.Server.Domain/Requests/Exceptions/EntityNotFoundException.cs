namespace Uploadify.Server.Domain.Requests.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string entityId, string entityName)
    {
        EntityID = entityId;
        EntityName = entityName;
    }

    public string EntityID { get; set; }
    public string EntityName { get; set; }
}
