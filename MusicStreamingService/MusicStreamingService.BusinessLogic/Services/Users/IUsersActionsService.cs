using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.BusinessLogic.Services.Users.Models;
using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.BusinessLogic.Services.Users;

public interface IUsersActionsService
{
    Task<UserSongModel> AddSongToAccountAsync(Guid userId, Guid songId);
    Task<UserSongModel> DeleteSongFromAccountAsync(Guid userId, Guid songId);
    Task<UserAlbumModel> AddAlbumToAccountAsync(Guid userId, Guid albumId);
    Task<UserAlbumModel> DeleteAlbumFromAccountAsync(Guid userId, Guid albumId);
    Task<CursorResponse<DateTime?, AlbumModel>> GetUserAlbumsByTitleAsync(Guid userId, string? titlePart,
        PaginationParams<DateTime?> paginationParams);
    Task<CursorResponse<DateTime?, SongModel>> GetUserSongsByNameAsync(Guid userId, string? namePart,
        PaginationParams<DateTime?> paginationParams);
    Task<List<SongModel>> GetListeningHistoryAsync(Guid userId, CancellationToken cancellationToken);
}