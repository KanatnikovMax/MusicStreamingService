using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.DataAccess.Postgres.Repositories.Interfaces;

public interface IArtistsRepository : IPgRepository<Artist>
{
    Task<CursorResponse<long?, Artist>> FindAllAsync(PaginationParams<long?> request);
    Task<IEnumerable<Album>> FindAllAlbumsAsync(Guid artistId);

    Task<IEnumerable<Song>> FindAllSongsAsync(Guid artistId);
    Task<Artist?> FindByNameAsync(string name);

    Task<CursorResponse<long?, Artist>> FindByNamePartAsync(string namePart, 
        PaginationParams<long?> request);

    Task<CursorResponse<DateTime?, Album>> FindAllAlbumsAsync(Guid artistId, PaginationParams<DateTime?> request);
    
    Task<CursorResponse<long?, Song>> FindAllSongsAsync(Guid artistId, PaginationParams<long?> request);
    
    Task<CursorResponse<long?, Song>> FindAllSongsByTitleAsync(Guid artistId, string titlePart, 
        PaginationParams<long?> request);

    Task<List<Artist>> GetOrCreateArtistsAsync(IEnumerable<string> names);
}
