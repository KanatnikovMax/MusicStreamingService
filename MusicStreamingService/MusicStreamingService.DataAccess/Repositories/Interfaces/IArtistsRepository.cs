using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.DataAccess.Repositories.Interfaces;

public interface IArtistsRepository : IPgRepository<Artist>
{
    Task<IEnumerable<Artist>> FindByNameAsync(string namePart);

    Task<IEnumerable<Album>> FindAllAlbumsAsync(Guid artistId);
    
    Task<IEnumerable<Song>> FindAllSongsAsync(Guid artistId);
    
    Task<IEnumerable<Song>> FindAllSongsByTitleAsync(Guid artistId, string titlePart);
}