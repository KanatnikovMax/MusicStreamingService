namespace MusicStreamingService.DataAccess.Entities;

public class Artist : BaseEntity
{
    public string Name { get; set; }
    
    public ICollection<Album>? Albums { get; set; }
    public ICollection<Song>? Songs { get; set; }
}