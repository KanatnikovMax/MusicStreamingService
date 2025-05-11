using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.DataAccess.Postgres.Repositories.Interfaces;

public interface IAlbumsRepository : IPgRepository<Album>
{
    Task<CursorResponse<DateTime?, Album>> FindAllAsync(PaginationParams<DateTime?> request);
    Task<Album?> FindByTitleAsync(string title);

    Task<CursorResponse<DateTime?, Album>> FindByTitlePartAsync(string titlePart, 
        PaginationParams<DateTime?> request);
    
    Task<CursorResponse<int?, Song>> FindAllSongsAsync(Guid albumId, PaginationParams<int?> request);
}