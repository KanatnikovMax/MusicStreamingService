namespace MusicStreamingService.Service.Controllers.Requests.Albums;

public record CreateAlbumRequest(
    string Title,
    DateTime ReleaseDate,
    List<string> Artists);