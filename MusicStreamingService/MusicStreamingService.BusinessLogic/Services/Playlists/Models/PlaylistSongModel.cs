namespace MusicStreamingService.BusinessLogic.Services.Playlists.Models;

public class PlaylistSongModel
{
    public Guid PlaylistId { get; set; }
    public Guid SongId { get; set; }
    public int Order { get; set; }
}
