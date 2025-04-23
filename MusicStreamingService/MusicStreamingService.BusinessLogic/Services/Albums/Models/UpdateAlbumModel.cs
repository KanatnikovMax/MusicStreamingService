namespace MusicStreamingService.BusinessLogic.Services.Albums.Models;

public record UpdateAlbumModel(
    string? Title,
    DateTime? ReleaseDate,
    List<string>? Artists);