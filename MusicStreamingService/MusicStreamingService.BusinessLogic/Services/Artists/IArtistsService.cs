using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Artists.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;

namespace MusicStreamingService.BusinessLogic.Services.Artists;

public interface IArtistsService
{
    Task<IEnumerable<ArtistModel>> GetAllArtistsAsync();
    Task<ArtistModel> GetArtistByIdAsync(Guid id);
    Task<IEnumerable<ArtistModel>> GetArtistByNameAsync(string namePart);
    Task<IEnumerable<AlbumModel>> GetAllAlbumsAsync(Guid artistId);
    Task<IEnumerable<SongModel>> GetAllSongsAsync(Guid artistId);

    Task<IEnumerable<SongModel>> GetSongsByTitleAsync(Guid artistId, string titlePart);
    Task<ArtistModel> CreateArtistAsync(CreateArtistModel model);
    Task<ArtistModel> DeleteArtistAsync(Guid id);

    Task<ArtistModel> UpdateArtistAsync(UpdateArtistModel model, Guid id);
}