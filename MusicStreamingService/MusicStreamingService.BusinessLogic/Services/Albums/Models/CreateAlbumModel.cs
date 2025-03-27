namespace MusicStreamingService.BusinessLogic.Services.Albums.Models;

public record CreateAlbumModel(
    string Title,
    DateTime ReleaseDate,
    List<string> Artists);