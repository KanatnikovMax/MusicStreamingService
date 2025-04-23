using MusicStreamingService.BusinessLogic.Services.Artists.Models;
using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.BusinessLogic.Services.Songs.Models;

public record SongModel(
    Guid Id, 
    string Title, 
    int Duration,
    int TrackNumber,
    Guid CassandraId,
    Guid AlbumId,
    List<ArtistSimpleModel> Artists);