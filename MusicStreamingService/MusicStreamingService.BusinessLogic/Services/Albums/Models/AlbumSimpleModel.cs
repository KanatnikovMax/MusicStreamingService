using MusicStreamingService.BusinessLogic.Services.Artists.Models;

namespace MusicStreamingService.BusinessLogic.Services.Albums.Models;

public record AlbumSimpleModel(
    Guid Id,
    string Title,
    DateTime ReleaseDate,
    List<ArtistSimpleModel> Artists);