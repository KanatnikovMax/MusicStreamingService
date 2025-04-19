using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.DataAccess.Context.Configuration;

public static class ArtistsContextConfigurator
{
    public static void ConfigureArtists(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Artist>().ToTable("artists");
        modelBuilder.Entity<Artist>().Property(a => a.Id)
            .HasDefaultValueSql("gen_random_uuid()");
        modelBuilder.Entity<Artist>().Property(a => a.CreatedAt)
            .HasDefaultValueSql("timezone('utc', now())");
        modelBuilder.Entity<Artist>().HasIndex(a => a.CreatedAt);
        modelBuilder.Entity<Artist>().Property(x => x.Name).IsRequired();
        modelBuilder.Entity<Artist>().Property(x => x.Name).HasMaxLength(50);
        modelBuilder.Entity<Artist>().HasIndex(x => x.Name).IsUnique();
        
        modelBuilder.Entity<Artist>().HasMany(x => x.Albums).WithMany(x => x.Artists)
            .UsingEntity(t => t.ToTable("artists_albums"));
        /*modelBuilder.Entity<Artist>().HasMany(x => x.Songs).WithMany(x => x.Artists)
            .UsingEntity(t => t.ToTable("artists_songs"));*/
        modelBuilder.Entity<Artist>().HasMany(x => x.Songs).WithMany(x => x.Artists)
            .UsingEntity<ArtistSong>(
                j => j.HasOne<Song>(x => x.Song).WithMany().OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne(x => x.Artist).WithMany().OnDelete(DeleteBehavior.Cascade),
                t => 
                {
                    t.ToTable("artists_songs");
                });
    }
}