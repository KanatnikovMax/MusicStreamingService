using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;

namespace MusicStreamingService.BusinessLogic.Services.Artists.Models;

public class ArtistModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? PhotoBase64 { get; set; }
    public List<SongSimpleModel> Songs { get; set; }
    public List<AlbumSimpleModel> Albums { get; set; }
}