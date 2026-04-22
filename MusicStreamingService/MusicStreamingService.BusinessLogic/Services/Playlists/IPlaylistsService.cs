using MusicStreamingService.BusinessLogic.Services.Playlists.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.BusinessLogic.Services.Playlists;

public interface IPlaylistsService
{
    Task<PlaylistModel> CreateAsync(Guid userId, CreatePlaylistModel model);
    Task<PlaylistModel> UpdateAsync(Guid userId, Guid playlistId, UpdatePlaylistModel model);
    Task<PlaylistModel> DeleteAsync(Guid userId, Guid playlistId);
    Task<PlaylistSongModel> AddSongAsync(Guid userId, Guid playlistId, Guid songId);
    Task<PlaylistSongModel> RemoveSongAsync(Guid userId, Guid playlistId, Guid songId);
    Task<CursorResponse<DateTime?, PlaylistModel>> GetUserPlaylistsAsync(Guid userId, string? namePart,
        PaginationParams<DateTime?> paginationParams);
    Task<CursorResponse<int?, SongModel>> GetPlaylistSongsAsync(Guid userId, Guid playlistId, string? namePart,
        PaginationParams<int?> paginationParams);
}
