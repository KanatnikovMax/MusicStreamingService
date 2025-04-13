using MusicStreamingService.BusinessLogic.Services.Albums.Models;

namespace MusicStreamingService.BusinessLogic.Services.Albums;

public interface IAlbumsService
{
    Task<IEnumerable<AlbumModel>> GetAllAlbumsAsync();
    Task<AlbumModel> GetAlbumByIdAsync(Guid id);
    Task<IEnumerable<AlbumModel>> GetAlbumByNameAsync(string titlePart);
    Task<AlbumModel> CreateAlbumAsync(CreateAlbumModel model);
    Task<AlbumModel> DeleteAlbumAsync(Guid id);

    Task<AlbumModel> UpdateAlbumAsync(UpdateAlbumModel model, Guid id);
}