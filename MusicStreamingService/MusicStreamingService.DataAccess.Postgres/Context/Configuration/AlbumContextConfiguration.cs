using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.DataAccess.Postgres.Context.Configuration;

public static class AlbumContextConfiguration
{
    public static void ConfigureAlbums(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Album>().ToTable("albums");
        modelBuilder.Entity<Album>().Property(a => a.Id)
            .HasDefaultValueSql("gen_random_uuid()");
        modelBuilder.Entity<Album>().Property(a => a.CreatedAt)
            .HasDefaultValueSql("timezone('utc', now())");
        modelBuilder.Entity<Album>().HasIndex(a => a.CreatedAt);
        modelBuilder.Entity<Album>().Property(x => x.Title).IsRequired();
        modelBuilder.Entity<Album>().Property(x => x.ReleaseDate).IsRequired();
        modelBuilder.Entity<Album>().HasIndex(x => x.Title);
        modelBuilder.Entity<Album>().Property(x => x.Title).HasMaxLength(50);
    }
}