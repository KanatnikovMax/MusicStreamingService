using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.DataAccess.Postgres.Repositories.Interfaces;

public interface ISongsRepository : IPgRepository<Song>
{
    Task<CursorResponse<PopularityCursor?, Song>> FindAllAsync(PaginationParams<PopularityCursor> request);
    Task<Song?> FindByTitleAsync(string title);
    Task<List<Song>> FindByIdsAsync(IEnumerable<Guid> ids);
    Task<CursorResponse<PopularityCursor?, Song>> FindByTitlePartAsync(string titlePart, 
        PaginationParams<PopularityCursor> request);
    Task<List<Guid>?> IncrementPlayCountAsync(Guid songId);
}
