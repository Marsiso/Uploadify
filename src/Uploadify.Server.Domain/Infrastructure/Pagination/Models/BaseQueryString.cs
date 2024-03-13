namespace Uploadify.Server.Domain.Infrastructure.Pagination.Models;

public class BaseQueryString
{
    public const int MaxPageSize = 100;
    public const int MinPageSize = 10;
    public const int MinPageNumber = 1;

    private int _pageSize = MinPageSize;
    private int _pageNumber = MinPageNumber;

    public int PageNumber
    {
        get => _pageNumber;
        set
        {
            if (value > MinPageNumber)
            {
                _pageNumber = value;
                return;
            }

            _pageNumber = MinPageNumber;
        }
    }

    public int PageSize
    {
        get => _pageSize;
        set
        {
            switch (value)
            {
                case > MaxPageSize:
                    _pageSize = MaxPageSize;
                    return;

                case < MinPageSize:
                    _pageSize = MinPageSize;
                    return;

                default:
                    _pageSize = value;
                    return;
            }
        }
    }

    public string? SearchTerm { get; set; }
}
