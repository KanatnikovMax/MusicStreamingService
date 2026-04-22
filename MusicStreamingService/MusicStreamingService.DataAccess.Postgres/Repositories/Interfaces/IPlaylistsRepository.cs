using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.DataAccess.Postgres.Repositories.Interfaces;

public interface IPlaylistsRepository : IPgRepository<Playlist>
{
    Task<CursorResponse<DateTime?, Playlist>> FindAllByUserIdAsync(Guid userId, string? namePart,
        PaginationParams<DateTime?> request);

    Task<Playlist?> FindUserPlaylistByIdAsync(Guid userId, Guid playlistId);

    Task<CursorResponse<int?, Song>> FindSongsAsync(Guid playlistId, string? namePart, PaginationParams<int?> request);

    Task<PlaylistSong?> AddSongAsync(Guid playlistId, Guid songId);
    Task<PlaylistSong?> FindSongAsync(Guid playlistId, Guid songId);

    void DeleteSong(PlaylistSong playlistSong);
}
