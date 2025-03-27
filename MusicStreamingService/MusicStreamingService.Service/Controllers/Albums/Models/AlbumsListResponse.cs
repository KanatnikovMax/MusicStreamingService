using MusicStreamingService.BusinessLogic.Services.Albums.Models;

namespace MusicStreamingService.Service.Controllers.Albums.Models;

public record AlbumsListResponse(List<AlbumModel> Albums);