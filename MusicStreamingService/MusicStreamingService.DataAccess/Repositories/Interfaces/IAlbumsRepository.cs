using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.DataAccess.Repositories.Interfaces;

public interface IAlbumsRepository : IPgRepository<Album>
{
    Task<Album?> FindByTitleAsync(string title);

    Task<IEnumerable<Album>> FindByTitlePartAsync(string titlePart);
    
    Task<IEnumerable<Song>> FindAllSongsAsync(Guid albumId);
}