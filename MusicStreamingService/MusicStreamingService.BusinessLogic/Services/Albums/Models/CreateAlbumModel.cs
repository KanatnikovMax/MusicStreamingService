namespace MusicStreamingService.BusinessLogic.Services.Albums.Models;

public class CreateAlbumModel
{
    public string Title { get; set; } 
    public byte[]? Photo { get; set; }
    public DateTime ReleaseDate { get; set; }
    public List<string> Artists { get; set; }
}