using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.DataAccess.Repositories.Interfaces;

public interface ISongsRepository : IPgRepository<Song>
{
    Task<PaginatedResponse<Song>> FindAllAsync(PaginationParams request);
    Task<Song?> FindByTitleAsync(string title);
    Task<PaginatedResponse<Song>> FindByTitlePartAsync(string titlePart, PaginationParams request);
}