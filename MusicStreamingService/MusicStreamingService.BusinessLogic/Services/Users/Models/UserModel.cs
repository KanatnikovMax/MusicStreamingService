using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;

namespace MusicStreamingService.BusinessLogic.Services.Users.Models;

public record UserModel(
    Guid Id,
    string UserName,
    string Email,
    List<SongSimpleModel> AddedSongs,
    List<AlbumSimpleModel> AddedAlbums);