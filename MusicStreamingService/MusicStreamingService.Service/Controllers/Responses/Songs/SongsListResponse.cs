using MusicStreamingService.BusinessLogic.Services.Songs.Models;

namespace MusicStreamingService.Service.Controllers.Responses.Songs;

public record SongsListResponse(List<SongModel> Songs);