using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.DataAccess.Postgres.Context.Configuration;

public static class UsersContextConfigurator
{
    public static void ConfigureUsers(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<User>().Property(u => u.Id)
            .HasDefaultValueSql("gen_random_uuid()");
        modelBuilder.Entity<User>().HasIndex(u => u.UserName).IsUnique();
        modelBuilder.Entity<Role>().ToTable("user_roles");
        modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("user_role_claims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims");
        
        modelBuilder.Entity<User>().HasMany(x => x.Albums).WithMany(x => x.Users)
            .UsingEntity<UserAlbum>(
                t => t
                    .HasOne(ua => ua.Album)
                    .WithMany(a => a.UsersAlbums)
                    .HasForeignKey(ua => ua.AlbumId)
                    .OnDelete(DeleteBehavior.Cascade),
                t => t
                    .HasOne(ua => ua.User)
                    .WithMany(u => u.UsersAlbums)
                    .HasForeignKey(ua => ua.UserId)
                    .OnDelete(DeleteBehavior.Cascade),
                t => 
                {
                    t.ToTable("users_albums");
                    t.Property(ua => ua.AddedTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
                });

        modelBuilder.Entity<User>().HasMany(x => x.Songs).WithMany(x => x.Users)
            .UsingEntity<UserSong>(
                t => t
                    .HasOne(us => us.Song)
                    .WithMany(s => s.UsersSongs)
                    .HasForeignKey(us => us.SongId)
                    .OnDelete(DeleteBehavior.Cascade),
                t => t
                    .HasOne(us => us.User)
                    .WithMany(u => u.UsersSongs)
                    .HasForeignKey(us => us.UserId)
                    .OnDelete(DeleteBehavior.Cascade),
                t =>
                {
                    t.ToTable("users_songs");
                    t.Property(us => us.AddedTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
                });
        
    }
}