using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.BusinessLogic.Services.Albums;

public interface IAlbumsService
{
    Task<PaginatedResponse<DateTime?, AlbumModel>> GetAllAlbumsAsync(PaginationParams<DateTime?> request);
    Task<AlbumModel> GetAlbumByIdAsync(Guid id);
    Task<PaginatedResponse<DateTime?, AlbumModel>> GetAlbumByTitleAsync(string titlePart, 
        PaginationParams<DateTime?> request);
    Task<PaginatedResponse<int?, SongModel>> GetAllAlbumSongsAsync(Guid albumId, PaginationParams<int?> request);
    Task<AlbumModel> CreateAlbumAsync(CreateAlbumModel model);
    Task<AlbumModel> DeleteAlbumAsync(Guid id);

    Task<AlbumModel> UpdateAlbumAsync(UpdateAlbumModel model, Guid id);
}