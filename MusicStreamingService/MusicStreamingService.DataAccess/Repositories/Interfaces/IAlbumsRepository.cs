using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.DataAccess.Repositories.Interfaces;

public interface IAlbumsRepository : IPgRepository<Album>
{
    Task<IEnumerable<Album>> FindByTitleAsync(string titlePart);
    
    Task<IEnumerable<Song>> FindAllSongsAsync(Guid albumId);
}