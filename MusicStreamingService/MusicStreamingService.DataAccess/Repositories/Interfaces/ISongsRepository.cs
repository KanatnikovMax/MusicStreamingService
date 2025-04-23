using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.DataAccess.Repositories.Interfaces;

public interface ISongsRepository : IPgRepository<Song>
{
    Task<CursorResponse<DateTime?, Song>> FindAllAsync(PaginationParams<DateTime?> request);
    Task<Song?> FindByTitleAsync(string title);
    Task<CursorResponse<DateTime?, Song>> FindByTitlePartAsync(string titlePart, 
        PaginationParams<DateTime?> request);
}