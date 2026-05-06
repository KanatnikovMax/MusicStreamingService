using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.DataAccess.Postgres.Repositories.Interfaces;

public interface ISongsRepository : IPgRepository<Song>
{
    Task<CursorResponse<long?, Song>> FindAllAsync(PaginationParams<long?> request);
    Task<Song?> FindByTitleAsync(string title);
    Task<List<Song>> FindByIdsAsync(IEnumerable<Guid> ids);
    Task<CursorResponse<long?, Song>> FindByTitlePartAsync(string titlePart, 
        PaginationParams<long?> request);
    Task<List<Guid>?> IncrementPlayCountAsync(Guid songId);
}
