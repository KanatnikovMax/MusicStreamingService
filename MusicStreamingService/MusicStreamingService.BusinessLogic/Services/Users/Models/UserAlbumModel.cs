namespace MusicStreamingService.BusinessLogic.Services.Users.Models;

public record UserAlbumModel(
    Guid UserId,
    Guid AlbumId);