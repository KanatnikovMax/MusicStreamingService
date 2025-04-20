using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.DataAccess.Repositories.Interfaces;

public interface ISongsRepository : IPgRepository<Song>
{
    Task<PaginatedResponse<DateTime?, Song>> FindAllAsync(PaginationParams<DateTime?> request);
    Task<Song?> FindByTitleAsync(string title);
    Task<PaginatedResponse<DateTime?, Song>> FindByTitlePartAsync(string titlePart, 
        PaginationParams<DateTime?> request);
}