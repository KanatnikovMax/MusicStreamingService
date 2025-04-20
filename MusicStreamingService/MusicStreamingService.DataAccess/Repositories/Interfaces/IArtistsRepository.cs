using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.DataAccess.Repositories.Interfaces;

public interface IArtistsRepository : IPgRepository<Artist>
{
    Task<PaginatedResponse<DateTime?, Artist>> FindAllAsync(PaginationParams<DateTime?> request);
    Task<IEnumerable<Album>> FindAllAlbumsAsync(Guid artistId);

    Task<IEnumerable<Song>> FindAllSongsAsync(Guid artistId);
    Task<Artist?> FindByNameAsync(string name);

    Task<PaginatedResponse<DateTime?, Artist>> FindByNamePartAsync(string namePart, 
        PaginationParams<DateTime?> request);

    Task<PaginatedResponse<DateTime?, Album>> FindAllAlbumsAsync(Guid artistId, PaginationParams<DateTime?> request);
    
    Task<PaginatedResponse<DateTime?, Song>> FindAllSongsAsync(Guid artistId, PaginationParams<DateTime?> request);
    
    Task<PaginatedResponse<DateTime?, Song>> FindAllSongsByTitleAsync(Guid artistId, string titlePart, 
        PaginationParams<DateTime?> request);

    Task<List<Artist>> GetOrCreateArtistsAsync(IEnumerable<string> names);
}