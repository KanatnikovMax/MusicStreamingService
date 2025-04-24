namespace MusicStreamingService.BusinessLogic.Services.Users.Models;

public record UserSongModel(
    Guid UserId,
    Guid SongId);