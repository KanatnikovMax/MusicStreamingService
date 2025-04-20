using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.DataAccess.Repositories.Interfaces;

public interface IAlbumsRepository : IPgRepository<Album>
{
    Task<PaginatedResponse<DateTime?, Album>> FindAllAsync(PaginationParams<DateTime?> request);
    Task<Album?> FindByTitleAsync(string title);

    Task<PaginatedResponse<DateTime?, Album>> FindByTitlePartAsync(string titlePart, 
        PaginationParams<DateTime?> request);
    
    Task<PaginatedResponse<int?, Song>> FindAllSongsAsync(Guid albumId, PaginationParams<int?> request);
}