using MusicStreamingService.BusinessLogic.Services.Albums.Models;

namespace MusicStreamingService.Service.Controllers.Responses.Albums;

public record AlbumsListResponse(List<AlbumModel> Albums);