using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.Service.Controllers.Songs.Models;

public record SongsListResponse(List<Song> Songs);