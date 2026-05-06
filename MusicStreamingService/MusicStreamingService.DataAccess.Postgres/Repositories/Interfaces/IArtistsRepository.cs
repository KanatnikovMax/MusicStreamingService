using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.DataAccess.Postgres.Repositories.Interfaces;

public interface IArtistsRepository : IPgRepository<Artist>
{
    Task<CursorResponse<PopularityCursor?, Artist>> FindAllAsync(PaginationParams<PopularityCursor> request);
    Task<IEnumerable<Album>> FindAllAlbumsAsync(Guid artistId);

    Task<IEnumerable<Song>> FindAllSongsAsync(Guid artistId);
    Task<Artist?> FindByNameAsync(string name);

    Task<CursorResponse<PopularityCursor?, Artist>> FindByNamePartAsync(string namePart, 
        PaginationParams<PopularityCursor> request);

    Task<CursorResponse<DateTime?, Album>> FindAllAlbumsAsync(Guid artistId, PaginationParams<DateTime?> request);
    
    Task<CursorResponse<PopularityCursor?, Song>> FindAllSongsAsync(Guid artistId, PaginationParams<PopularityCursor> request);
    
    Task<CursorResponse<PopularityCursor?, Song>> FindAllSongsByTitleAsync(Guid artistId, string titlePart, 
        PaginationParams<PopularityCursor> request);

    Task<List<Artist>> GetOrCreateArtistsAsync(IEnumerable<string> names);
}
