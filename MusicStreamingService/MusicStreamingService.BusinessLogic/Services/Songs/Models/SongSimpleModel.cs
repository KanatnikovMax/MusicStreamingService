using MusicStreamingService.BusinessLogic.Services.Artists.Models;

namespace MusicStreamingService.BusinessLogic.Services.Songs.Models;

public record SongSimpleModel(
    Guid Id,
    string Title,
    List<ArtistSimpleModel> Artists,
    int TrackNumber,
    int Duration);