using MusicStreamingService.BusinessLogic.Services.Artists.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;

namespace MusicStreamingService.BusinessLogic.Services.Albums.Models;

public record AlbumModel(
    Guid Id,
    string Title,
    DateTime ReleaseDate,
    List<SongSimpleModel> Songs,
    List<ArtistSimpleModel> Artists);