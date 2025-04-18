namespace MusicStreamingService.BusinessLogic.Services.Songs.Models;

public record UpdateSongModel(
    string? Title, 
    int? TrackNumber);