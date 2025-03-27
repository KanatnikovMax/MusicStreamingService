namespace MusicStreamingService.Service.Controllers.Songs.Models;

public record UpdateSongRequest(
    string? Title, 
    int? Duration,
    int? TrackNumber, 
    string? AlbumTitle,
    List<string>? Artists);