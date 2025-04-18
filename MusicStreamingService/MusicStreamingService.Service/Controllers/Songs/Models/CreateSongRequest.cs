namespace MusicStreamingService.Service.Controllers.Songs.Models;

public record CreateSongRequest(
    string Title, 
    int Duration,
    int TrackNumber, 
    Guid AlbumId,
    List<string> Artists);