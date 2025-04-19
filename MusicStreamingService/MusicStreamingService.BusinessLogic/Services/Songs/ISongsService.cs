using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.BusinessLogic.Services.Songs;

public interface ISongsService
{
    Task<PaginatedResponse<SongModel>> GetAllSongsAsync(PaginationParams request);
    Task<SongModel> GetSongByIdAsync(Guid id);
    Task<PaginatedResponse<SongModel>> GetSongByTitleAsync(string titlePart, PaginationParams request);
    Task<SongModel> CreateSongAsync(CreateSongModel model);
    Task<SongModel> DeleteSongAsync(Guid id);

    Task<SongModel> UpdateSongAsync(UpdateSongModel model, Guid id);
}