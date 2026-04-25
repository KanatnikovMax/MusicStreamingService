using MusicStreamingService.BusinessLogic.Services.Artists.Models;

namespace MusicStreamingService.BusinessLogic.Services.Songs.Models;

public record SongModel(
    Guid Id, 
    string Title, 
    int Duration,
    int TrackNumber,
    string AudioObjectKey,
    Guid AlbumId,
    List<ArtistSimpleModel> Artists);