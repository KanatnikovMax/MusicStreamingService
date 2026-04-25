using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.DataAccess.Postgres.Context.Configuration;

public static class PlaylistsContextConfigurator
{
    public static void ConfigurePlaylists(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Playlist>().ToTable("playlists");
        modelBuilder.Entity<Playlist>().Property(p => p.Id)
            .HasDefaultValueSql("gen_random_uuid()");
        modelBuilder.Entity<Playlist>().Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();
        modelBuilder.Entity<Playlist>().Property(p => p.PhotoObjectKey)
            .HasMaxLength(512);
        modelBuilder.Entity<Playlist>().Property(p => p.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        modelBuilder.Entity<Playlist>().Property(p => p.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        modelBuilder.Entity<Playlist>()
            .HasIndex(p => new { p.UserId, p.Name });

        modelBuilder.Entity<Playlist>()
            .HasIndex(x => x.Name)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops")
            .HasDatabaseName("ix_playlists_name_trgm");
        
        modelBuilder.Entity<Playlist>()
            .HasOne(p => p.User)
            .WithMany(u => u.Playlists)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PlaylistSong>().ToTable("playlists_songs");
        modelBuilder.Entity<PlaylistSong>()
            .HasKey(ps => new { ps.PlaylistId, ps.SongId });
        modelBuilder.Entity<PlaylistSong>()
            .Property(ps => ps.AddedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        modelBuilder.Entity<PlaylistSong>()
            .HasIndex(ps => new { ps.PlaylistId, ps.Order })
            .IsUnique();

        modelBuilder.Entity<PlaylistSong>()
            .HasOne(ps => ps.Playlist)
            .WithMany(p => p.PlaylistSongs)
            .HasForeignKey(ps => ps.PlaylistId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PlaylistSong>()
            .HasOne(ps => ps.Song)
            .WithMany(s => s.PlaylistSongs)
            .HasForeignKey(ps => ps.SongId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
