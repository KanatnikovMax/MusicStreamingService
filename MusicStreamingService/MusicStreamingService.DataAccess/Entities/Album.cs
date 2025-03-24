namespace MusicStreamingService.DataAccess.Entities;

public class Album : BaseEntity
{
    public string Title { get; set; }
    public DateTime ReleaseDate { get; set; }
    
    public ICollection<User>? Users { get; set; }
    public ICollection<Song>? Songs { get; set; }
    public ICollection<Artist> Artists { get; set; }
    
    public ICollection<UserAlbum>? UsersAlbums { get; set; }
}