using MusicStreamingService.BusinessLogic.Services.Artists.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;

namespace MusicStreamingService.BusinessLogic.Services.Albums.Models;

public class AlbumModel
{
    public Guid Id { get; set; } 
    public string Title { get; set; } 
    public string? PhotoBase64 { get; set; }
    public DateTime ReleaseDate { get; set; } 
    public List<SongSimpleModel> Songs { get; set; }
    public List<ArtistSimpleModel> Artists { get; set; } 
}