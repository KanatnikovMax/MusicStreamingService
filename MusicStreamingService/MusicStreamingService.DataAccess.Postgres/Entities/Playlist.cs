namespace MusicStreamingService.DataAccess.Postgres.Entities;

public class Playlist : BaseEntity
{
    public string Name { get; set; }
    public byte[]? Photo { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<PlaylistSong>? PlaylistSongs { get; set; }
}
