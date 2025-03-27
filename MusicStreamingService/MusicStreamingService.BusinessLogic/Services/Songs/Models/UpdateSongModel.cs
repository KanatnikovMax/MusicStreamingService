namespace MusicStreamingService.BusinessLogic.Services.Songs.Models;

public record UpdateSongModel(
    string? Title, 
    int? Duration,
    int? TrackNumber, 
    string? AlbumTitle,
    List<string>? Artists);