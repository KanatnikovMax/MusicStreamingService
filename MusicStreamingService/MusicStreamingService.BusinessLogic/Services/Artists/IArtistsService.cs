using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Artists.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.BusinessLogic.Services.Artists;

public interface IArtistsService
{
    Task<PaginatedResponse<ArtistModel>> GetAllArtistsAsync(PaginationParams request);
    Task<ArtistModel> GetArtistByIdAsync(Guid id);
    Task<PaginatedResponse<ArtistModel>> GetArtistByNameAsync(string namePart, PaginationParams request);
    Task<PaginatedResponse<AlbumModel>> GetAllAlbumsAsync(Guid artistId, PaginationParams request);
    Task<PaginatedResponse<SongModel>> GetAllSongsAsync(Guid artistId, PaginationParams request);

    Task<PaginatedResponse<SongModel>> GetSongsByTitleAsync(Guid artistId, string titlePart, PaginationParams request);
    Task<ArtistModel> CreateArtistAsync(CreateArtistModel model);
    Task<ArtistModel> DeleteArtistAsync(Guid id);

    Task<ArtistModel> UpdateArtistAsync(UpdateArtistModel model, Guid id);
}