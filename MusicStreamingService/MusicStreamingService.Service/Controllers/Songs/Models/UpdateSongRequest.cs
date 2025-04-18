namespace MusicStreamingService.Service.Controllers.Songs.Models;

public record UpdateSongRequest(
    string? Title, 
    int? TrackNumber);