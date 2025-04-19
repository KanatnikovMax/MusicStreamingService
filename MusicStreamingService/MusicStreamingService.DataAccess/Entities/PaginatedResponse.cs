namespace MusicStreamingService.DataAccess.Entities;

public class PaginatedResponse<T>
{
    public DateTime? Cursor { get; set; }
    public List<T> Items { get; set; }
}