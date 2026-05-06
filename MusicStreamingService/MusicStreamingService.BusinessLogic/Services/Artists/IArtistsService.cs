using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Artists.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.BusinessLogic.Services.Artists;

public interface IArtistsService
{
    Task<ArtistModel> GetArtistByIdAsync(Guid id);
    Task<CursorResponse<PopularityCursor?, ArtistModel>> GetArtistByNameAsync(string? namePart, 
        PaginationParams<PopularityCursor> request);
    Task<CursorResponse<DateTime?, AlbumModel>> GetAllAlbumsAsync(Guid artistId, 
        PaginationParams<DateTime?> request);

    Task<CursorResponse<PopularityCursor?, SongModel>> GetSongsByTitleAsync(Guid artistId, string? titlePart, 
        PaginationParams<PopularityCursor> request);
    Task<ArtistModel> CreateArtistAsync(CreateArtistModel model);
    Task<ArtistModel> DeleteArtistAsync(Guid id);

    Task<ArtistModel> UpdateArtistAsync(UpdateArtistModel model, Guid id);
}
