namespace MusicStreamingService.Service.Controllers.Albums.Models;

public record CreateAlbumRequest(
    string Title,
    DateTime ReleaseDate,
    List<string> Artists);