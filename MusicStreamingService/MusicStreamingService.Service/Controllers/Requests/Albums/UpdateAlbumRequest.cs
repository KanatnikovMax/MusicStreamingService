namespace MusicStreamingService.Service.Controllers.Requests.Albums;

public record UpdateAlbumRequest(
    string? Title,
    DateTime? ReleaseDate,
    List<string>? Artists);