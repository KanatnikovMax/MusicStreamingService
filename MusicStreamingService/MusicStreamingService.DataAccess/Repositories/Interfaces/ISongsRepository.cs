using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.DataAccess.Repositories.Interfaces;

public interface ISongsRepository : IPgRepository<Song>
{
    Task<Song?> SaveAsync(Song entity);
    Task<Song?> FindByTitleAsync(string title);
    Task<IEnumerable<Song>> FindByTitlePartAsync(string titlePart);
}