using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.DataAccess.Repositories.Interfaces;

public interface IAlbumsRepository : IPgRepository<Album>
{
    Task<CursorResponse<DateTime?, Album>> FindAllAsync(PaginationParams<DateTime?> request);
    Task<Album?> FindByTitleAsync(string title);

    Task<CursorResponse<DateTime?, Album>> FindByTitlePartAsync(string titlePart, 
        PaginationParams<DateTime?> request);
    
    Task<CursorResponse<int?, Song>> FindAllSongsAsync(Guid albumId, PaginationParams<int?> request);
}