﻿using Microsoft.AspNetCore.Identity;

namespace MusicStreamingService.DataAccess.Postgres.Entities;

public class User : IdentityUser<Guid>
{
    public ICollection<Album>? Albums { get; set; }
    public ICollection<Song>? Songs { get; set; }
    
    public ICollection<UserAlbum>? UsersAlbums { get; set; }
    
    public ICollection<UserSong>? UsersSongs { get; set; }
    
    public ICollection<UserRole>? Roles { get; set; }
}

public class Role : IdentityRole<Guid>
{
    
}