using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Artists.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.BusinessLogic.Services.Artists;

public interface IArtistsService
{
    Task<CursorResponse<DateTime?, ArtistModel>> GetAllArtistsAsync(PaginationParams<DateTime?> request);
    Task<ArtistModel> GetArtistByIdAsync(Guid id);
    Task<CursorResponse<DateTime?, ArtistModel>> GetArtistByNameAsync(string namePart, 
        PaginationParams<DateTime?> request);
    Task<CursorResponse<DateTime?, AlbumModel>> GetAllAlbumsAsync(Guid artistId, 
        PaginationParams<DateTime?> request);
    Task<CursorResponse<DateTime?, SongModel>> GetAllSongsAsync(Guid artistId, PaginationParams<DateTime?> request);

    Task<CursorResponse<DateTime?, SongModel>> GetSongsByTitleAsync(Guid artistId, string titlePart, 
        PaginationParams<DateTime?> request);
    Task<ArtistModel> CreateArtistAsync(CreateArtistModel model);
    Task<ArtistModel> DeleteArtistAsync(Guid id);

    Task<ArtistModel> UpdateArtistAsync(UpdateArtistModel model, Guid id);
}