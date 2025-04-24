using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Context.Configuration;
using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.DataAccess.Context;

public class MusicServiceDbContext : IdentityDbContext<
    User, 
    Role, 
    Guid, 
    IdentityUserClaim<Guid>, 
    UserRole, 
    IdentityUserLogin<Guid>, 
    IdentityRoleClaim<Guid>, 
    IdentityUserToken<Guid>
>
{
    public DbSet<Album> Albums { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserSong> UsersSongs { get; set; }
    public DbSet<UserAlbum> UsersAlbums { get; set; }
    
    public MusicServiceDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureAlbums();
        modelBuilder.ConfigureArtists();
        modelBuilder.ConfigureSongs();
        modelBuilder.ConfigureUsers();
    }
}