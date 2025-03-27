namespace MusicStreamingService.Service.Controllers.Albums.Models;

public record UpdateAlbumRequest(
    string? Title,
    DateTime? ReleaseDate,
    List<string>? Artists);