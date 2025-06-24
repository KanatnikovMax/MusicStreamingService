using MusicStreamingService.BusinessLogic.Services.Artists.Models;

namespace MusicStreamingService.BusinessLogic.Services.Albums.Models;

public class AlbumSimpleModel
{
    public Guid Id { get; init; } 
    public string Title { get; init; } 
    public string? PhotoBase64 { get; set; }
    public DateTime ReleaseDate { get; init; } 
    public List<ArtistSimpleModel> Artists { get; init; } 
}