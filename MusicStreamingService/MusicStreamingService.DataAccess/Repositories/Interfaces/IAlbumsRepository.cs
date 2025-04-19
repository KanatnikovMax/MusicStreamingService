using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.DataAccess.Repositories.Interfaces;

public interface IAlbumsRepository : IPgRepository<Album>
{
    Task<PaginatedResponse<Album>> FindAllAsync(PaginationParams request);
    Task<Album?> FindByTitleAsync(string title);

    Task<PaginatedResponse<Album>> FindByTitlePartAsync(string titlePart, PaginationParams request);
    
    Task<PaginatedResponse<Song>> FindAllSongsAsync(Guid albumId, PaginationParams request);
}