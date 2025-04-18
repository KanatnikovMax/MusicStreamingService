using MusicStreamingService.BusinessLogic.Services.Songs.Models;

namespace MusicStreamingService.BusinessLogic.Services.Songs;

public interface ISongsService
{
    Task<IEnumerable<SongModel>> GetAllSongsAsync();
    Task<SongModel> GetSongByIdAsync(Guid id);
    Task<IEnumerable<SongModel>> GetSongByNameAsync(string titlePart);
    Task<SongModel> CreateSongAsync(CreateSongModel model);
    Task<SongModel> DeleteSongAsync(Guid id);

    Task<SongModel> UpdateSongAsync(UpdateSongModel model, Guid id);
}