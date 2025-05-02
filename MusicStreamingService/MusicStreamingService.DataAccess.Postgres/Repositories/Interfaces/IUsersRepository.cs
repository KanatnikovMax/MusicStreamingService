using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.DataAccess.Postgres.Repositories.Interfaces;

public interface IUsersRepository : IPgRepository<User>
{
    Task<IEnumerable<User>> FindByEmailAsync(string email);
    
    Task<CursorResponse<DateTime?, Album>> FindAllAlbumsAsync(Guid userId, PaginationParams<DateTime?> request);
    Task<CursorResponse<DateTime?, Album>> FindAllAlbumsByTitleAsync(Guid userId, string titlePart, 
        PaginationParams<DateTime?> request);
    Task<CursorResponse<DateTime?, Song>> FindAllSongsAsync(Guid userId, PaginationParams<DateTime?> request);
    Task<CursorResponse<DateTime?, Song>> FindAllSongsByNameAsync(Guid userId, string namePart,
        PaginationParams<DateTime?> request);
    Task<UserAlbum?> AddAlbumAsync(Guid userId, Guid albumId);
    
    Task<UserSong> AddSongAsync(Guid userId, Guid songId);
    Task<UserAlbum?> FindAlbumByIdAsync(Guid userId, Guid albumId);
    Task<UserSong?> FindSongByIdAsync(Guid userId, Guid songId);
    void DeleteAlbum(UserAlbum album);
    
    void DeleteSong(UserSong song);
}