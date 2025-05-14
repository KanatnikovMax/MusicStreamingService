using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.DataAccess.Postgres.Repositories.Interfaces;

public interface IArtistsRepository : IPgRepository<Artist>
{
    Task<CursorResponse<DateTime?, Artist>> FindAllAsync(PaginationParams<DateTime?> request);
    Task<IEnumerable<Album>> FindAllAlbumsAsync(Guid artistId);

    Task<IEnumerable<Song>> FindAllSongsAsync(Guid artistId);
    Task<Artist?> FindByNameAsync(string name);

    Task<CursorResponse<DateTime?, Artist>> FindByNamePartAsync(string namePart, 
        PaginationParams<DateTime?> request);

    Task<CursorResponse<DateTime?, Album>> FindAllAlbumsAsync(Guid artistId, PaginationParams<DateTime?> request);
    
    Task<CursorResponse<DateTime?, Song>> FindAllSongsAsync(Guid artistId, PaginationParams<DateTime?> request);
    
    Task<CursorResponse<DateTime?, Song>> FindAllSongsByTitleAsync(Guid artistId, string titlePart, 
        PaginationParams<DateTime?> request);

    Task<List<Artist>> GetOrCreateArtistsAsync(IEnumerable<string> names);
}