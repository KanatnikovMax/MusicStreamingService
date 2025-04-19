using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.DataAccess.Repositories.Interfaces;

public interface IArtistsRepository : IPgRepository<Artist>
{
    Task<PaginatedResponse<Artist>> FindAllAsync(PaginationParams request);
    Task<IEnumerable<Album>> FindAllAlbumsAsync(Guid artistId);

    Task<IEnumerable<Song>> FindAllSongsAsync(Guid artistId);
    Task<Artist?> FindByNameAsync(string name);

    Task<PaginatedResponse<Artist>> FindByNamePartAsync(string namePart, PaginationParams request);

    Task<PaginatedResponse<Album>> FindAllAlbumsAsync(Guid artistId, PaginationParams request);
    
    Task<PaginatedResponse<Song>> FindAllSongsAsync(Guid artistId, PaginationParams request);
    
    Task<PaginatedResponse<Song>> FindAllSongsByTitleAsync(Guid artistId, string titlePart, PaginationParams request);

    Task<List<Artist>> GetOrCreateArtistsAsync(IEnumerable<string> names);
}