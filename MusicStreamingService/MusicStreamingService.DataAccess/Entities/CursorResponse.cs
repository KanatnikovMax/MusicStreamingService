namespace MusicStreamingService.DataAccess.Entities;

public class CursorResponse<TCursor, TItems>
{
    public TCursor? Cursor { get; set; }
    public List<TItems> Items { get; set; }
}