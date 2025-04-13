using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.DataAccess.Repositories.Interfaces;

public interface IUsersRepository : IPgRepository<User>
{
    Task<User?> SaveAsync(User entity);
    Task<IEnumerable<User>> FindByEmailAsync(string email);
    
    Task<IEnumerable<Album>> FindAllAlbumsAsync(Guid userId);
    
    Task<IEnumerable<Album>> FindAllAlbumsByTitleAsync(Guid userId, string titlePart);
    
    Task<IEnumerable<Song>> FindAllSongsAsync(Guid userId);
    
    Task<IEnumerable<Song>> FindAllSongsByTitleAsync(Guid userId, string titlePart);
    
    Task<UserAlbum> AddAlbumAsync(Guid userId, Guid albumId);
    
    Task<UserSong> AddSongAsync(Guid userId, Guid songId);
}