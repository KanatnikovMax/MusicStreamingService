using Microsoft.AspNetCore.Identity;

namespace MusicStreamingService.DataAccess.Entities;

public class User : IdentityUser<Guid>
{
    public ICollection<Album>? Albums { get; set; }
    public ICollection<Song>? Songs { get; set; }
}

public class Role : IdentityRole<Guid>
{
    
}