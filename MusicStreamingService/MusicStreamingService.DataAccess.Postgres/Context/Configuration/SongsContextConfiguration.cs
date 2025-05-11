using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.DataAccess.Postgres.Context.Configuration;

public static class SongsContextConfiguration
{
    public static void ConfigureSongs(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Song>().ToTable("songs");
        modelBuilder.Entity<Song>().Property(s => s.Id)
            .HasDefaultValueSql("gen_random_uuid()");
        modelBuilder.Entity<Song>().Property(s => s.CreatedAt).HasDefaultValueSql("timezone('utc', now())");
        modelBuilder.Entity<Song>().HasIndex(s => s.CreatedAt);
        modelBuilder.Entity<Song>().Property(x => x.Title).IsRequired();
        modelBuilder.Entity<Song>().Property(x => x.CassandraId).IsRequired();
        modelBuilder.Entity<Song>().Property(x => x.Duration).IsRequired();
        modelBuilder.Entity<Song>().Property(x => x.TrackNumber).IsRequired();
        modelBuilder.Entity<Song>().HasIndex(x => x.Title);
        modelBuilder.Entity<Song>().Property(x => x.Title).HasMaxLength(50);
        modelBuilder.Entity<Song>().HasIndex(x => x.AlbumId);

        modelBuilder.Entity<Song>().HasOne(x => x.Album).WithMany(x => x.Songs)
            .HasForeignKey(x => x.AlbumId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}