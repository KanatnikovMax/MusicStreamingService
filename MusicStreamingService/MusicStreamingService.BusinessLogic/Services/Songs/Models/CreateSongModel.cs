namespace MusicStreamingService.BusinessLogic.Services.Songs.Models;

public record CreateSongModel(
    string Title, 
    int Duration,
    int TrackNumber, 
    Guid AlbumId,
    List<string> Artists);