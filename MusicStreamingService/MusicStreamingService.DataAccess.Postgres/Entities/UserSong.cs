namespace MusicStreamingService.DataAccess.Postgres.Entities;

public class UserSong
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public Guid SongId { get; set; }
    public Song Song { get; set; }
    
    public DateTime AddedTime { get; set; }
}