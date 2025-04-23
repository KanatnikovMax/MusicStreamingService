namespace MusicStreamingService.DataAccess.Entities;

public class ArtistSong
{
    public Guid ArtistId { get; set; }
    public Artist Artist { get; set; }

    public Guid SongId { get; set; }
    public Song Song { get; set; }
}