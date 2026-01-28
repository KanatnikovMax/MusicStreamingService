namespace MusicStreamingService.DataAccess.Postgres.Entities;

public class Artist : BaseEntity
{
    public string Name { get; set; }
    public byte[]? Photo { get; set; }
    public ICollection<Album>? Albums { get; set; }
    public ICollection<Song>? Songs { get; set; }
}