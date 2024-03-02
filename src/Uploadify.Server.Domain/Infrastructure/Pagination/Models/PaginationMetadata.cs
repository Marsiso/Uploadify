namespace Uploadify.Server.Domain.Infrastructure.Pagination.Models;

public abstract class PaginationMetadata
{
    protected PaginationMetadata(int totalItems, int pageNumber, int pageSize)
    {
        PageSize = pageSize;
        PageNumber = pageNumber;
        TotalItems = totalItems;
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
    }

    public int PageSize { get; private set; }
    public int PageNumber { get; private set; }
    public int TotalPages { get; private set; }
    public int TotalItems { get; private set; }

    public bool HasPrevious => PageNumber > BaseQueryString.MinPageNumber;
    public bool HasNext => PageNumber < TotalPages;
}
