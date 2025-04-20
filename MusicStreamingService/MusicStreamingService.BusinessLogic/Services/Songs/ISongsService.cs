using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.BusinessLogic.Services.Songs;

public interface ISongsService
{
    Task<PaginatedResponse<DateTime?, SongModel>> GetAllSongsAsync(PaginationParams<DateTime?> request);
    Task<SongModel> GetSongByIdAsync(Guid id);
    Task<PaginatedResponse<DateTime?, SongModel>> GetSongByTitleAsync(string titlePart, 
        PaginationParams<DateTime?> request);
    Task<SongModel> CreateSongAsync(CreateSongModel model);
    Task<SongModel> DeleteSongAsync(Guid id);

    Task<SongModel> UpdateSongAsync(UpdateSongModel model, Guid id);
}