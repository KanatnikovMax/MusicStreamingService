using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;

namespace MusicStreamingService.BusinessLogic.Services.Artists.Models;

public record ArtistModel(
    Guid Id,
    string Name,
    List<SongSimpleModel> Songs,
    List<AlbumSimpleModel> Albums);