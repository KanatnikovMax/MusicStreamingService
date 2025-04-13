using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.DataAccess.Repositories.Interfaces;

public interface IArtistsRepository : IPgRepository<Artist>
{
    Task<Artist?> SaveAsync(Artist entity);
    Task<Artist?> FindByNameAsync(string name);

    Task<IEnumerable<Artist>> FindByNamePartAsync(string namePart);

    Task<IEnumerable<Album>> FindAllAlbumsAsync(Guid artistId);
    
    Task<IEnumerable<Song>> FindAllSongsAsync(Guid artistId);
    
    Task<IEnumerable<Song>> FindAllSongsByTitleAsync(Guid artistId, string titlePart);
}