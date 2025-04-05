using MusicStreamingService.BusinessLogic.Services.Songs.Models;

namespace MusicStreamingService.Service.Controllers.Songs.Models;

public record SongsListResponse(List<SongModel> Songs);