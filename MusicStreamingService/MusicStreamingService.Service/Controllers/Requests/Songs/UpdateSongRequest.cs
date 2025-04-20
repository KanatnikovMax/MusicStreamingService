namespace MusicStreamingService.Service.Controllers.Requests.Songs;

public record UpdateSongRequest(
    string? Title, 
    int? TrackNumber);