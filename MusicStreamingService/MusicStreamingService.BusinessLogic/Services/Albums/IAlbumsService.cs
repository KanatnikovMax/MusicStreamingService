using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.BusinessLogic.Services.Albums;

public interface IAlbumsService
{
    Task<PaginatedResponse<AlbumModel>> GetAllAlbumsAsync(PaginationParams request);
    Task<AlbumModel> GetAlbumByIdAsync(Guid id);
    Task<PaginatedResponse<AlbumModel>> GetAlbumByTitleAsync(string titlePart, PaginationParams request);
    Task<PaginatedResponse<SongModel>> GetAllAlbumSongsAsync(Guid albumId, PaginationParams request);
    Task<AlbumModel> CreateAlbumAsync(CreateAlbumModel model);
    Task<AlbumModel> DeleteAlbumAsync(Guid id);

    Task<AlbumModel> UpdateAlbumAsync(UpdateAlbumModel model, Guid id);
}