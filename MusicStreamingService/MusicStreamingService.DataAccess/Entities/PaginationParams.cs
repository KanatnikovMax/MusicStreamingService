namespace MusicStreamingService.DataAccess.Entities;

public class PaginationParams
{
    private const int MaxPageSize = 100; 
    private int _pageSize = 10;

    public DateTime? Cursor { get; set; }
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
}