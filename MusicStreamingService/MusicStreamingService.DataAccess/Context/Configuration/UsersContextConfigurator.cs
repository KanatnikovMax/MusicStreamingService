using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.DataAccess.Context.Configuration;

public static class UsersContextConfigurator
{
    public static void ConfigureUsers(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<Role>().ToTable("user_roles");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens").HasNoKey();
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("user_role_owners").HasNoKey();
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("user_role_claims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins").HasNoKey();
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims");
        
        
        modelBuilder.Entity<User>().HasMany(x => x.Albums).WithMany(x => x.Users)
            .UsingEntity(t => t.ToTable("users_albums"));
        modelBuilder.Entity<User>().HasMany(x => x.Songs).WithMany(x => x.Users)
            .UsingEntity(t => t.ToTable("users_songs"));
    }
}