using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.DataAccess.Context.Configuration;

public static class AlbumContextConfiguration
{
    public static void ConfigureAlbums(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Album>().ToTable("albums");
        modelBuilder.Entity<Album>().Property(x => x.Title).IsRequired();
        modelBuilder.Entity<Album>().Property(x => x.ReleaseDate).IsRequired();
        modelBuilder.Entity<Album>().HasIndex(x => x.Title);
        modelBuilder.Entity<Album>().Property(x => x.Title).HasMaxLength(50);
    }
}