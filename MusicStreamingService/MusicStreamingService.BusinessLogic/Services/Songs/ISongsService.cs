using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.BusinessLogic.Services.Songs;

public interface ISongsService
{
    Task<SongModel> GetSongByIdAsync(Guid id);
    Task<CursorResponse<PopularityCursor?, SongModel>> GetSongByTitleAsync(string? titlePart, 
        PaginationParams<PopularityCursor> request);
    Task<SongModel> CreateSongAsync(CreateSongModel model, byte[] audioData);
    Task<SongModel> DeleteSongAsync(Guid id);

    Task<SongModel> UpdateSongAsync(UpdateSongModel model, Guid id);
    Task<string?> GetSongAudioUrlAsync(Guid id);
    Task RecordSongPlayedAsync(Guid songId);
}
