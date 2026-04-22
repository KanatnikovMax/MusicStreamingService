namespace MusicStreamingService.DataAccess.Postgres.Entities;

public class PlaylistSong
{
    public Guid PlaylistId { get; set; }
    public Playlist Playlist { get; set; }

    public Guid SongId { get; set; }
    public Song Song { get; set; }

    public DateTime AddedAt { get; set; }
    public int Order { get; set; }
}
