namespace MusicStreamingService.DataAccess.Postgres.Entities;

public class PaginationParams<T>
{
    private const int MaxPageSize = 100; 
    private int _pageSize = 10;

    public T? Cursor { get; set; }
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : (value == 0 ? _pageSize : value);
    }
}