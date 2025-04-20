namespace MusicStreamingService.Service.Controllers.Requests.Songs;

public record CreateSongRequest(
    string Title, 
    int Duration,
    int TrackNumber, 
    Guid AlbumId,
    List<string> Artists);