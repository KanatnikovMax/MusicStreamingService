namespace MusicStreamingService.DataAccess.Postgres.Entities;

public class UserAlbum
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public Guid AlbumId { get; set; }
    public Album Album { get; set; }
    
    public DateTime AddedTime { get; set; }
}