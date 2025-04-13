using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Context;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.DataAccess.Repositories.Interfaces;

namespace MusicStreamingService.DataAccess.Repositories;

public class UsersRepository : IUsersRepository
{
     private readonly IDbContextFactory<MusicServiceDbContext> _dbContextFactory;

    public UsersRepository(IDbContextFactory<MusicServiceDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<User>> FindAllAsync()
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<User>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> FindAllAsync(Expression<Func<User, bool>> predicate)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<User>()
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<User?> FindByIdAsync(Guid id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task DeleteAsync(User entity)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        context.Set<User>().Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<User?> SaveAsync(User entity)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var user = context.Set<User>().FirstOrDefault(a => a.Id == entity.Id);
        
        if (user is not null) 
            return null;
        
        var result = await context.Set<User>().AddAsync(entity);
        await context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<User> UpdateAsync(User entity)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var result = context.Set<User>().Attach(entity);
        context.Entry(entity).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<IEnumerable<User>> FindByEmailAsync(string email)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<User>()
            .AsNoTracking()
            .Where(a => a.Email == email)
            .ToListAsync();
    }

    public async Task<IEnumerable<Album>> FindAllAlbumsAsync(Guid userId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<UserAlbum>()
            .Where(ua => ua.UserId == userId)
            .Include(ua => ua.Album)
            .OrderByDescending(ua => ua.AddedTime)
            .Select(ua => ua.Album)
            .ToListAsync();
    }

    public async Task<IEnumerable<Album>> FindAllAlbumsByTitleAsync(Guid userId, string titlePart)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<UserAlbum>()
            .Where(ua => ua.UserId == userId)
            .Include(ua => ua.Album)
            .OrderByDescending(ua => ua.AddedTime)
            .Select(ua => ua.Album)
            .Where(a => EF.Functions.ILike(a.Title, $"{titlePart}%"))
            .ToListAsync(); 
    }

    public async Task<IEnumerable<Song>> FindAllSongsAsync(Guid userId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<UserSong>()
            .Where(us => us.UserId == userId)
            .Include(us => us.Song)
            .OrderByDescending(us => us.AddedTime)
            .Select(us => us.Song)
            .ToListAsync();
    }

    public async Task<IEnumerable<Song>> FindAllSongsByTitleAsync(Guid userId, string titlePart)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<UserSong>()
            .Where(us => us.UserId == userId)
            .Include(us => us.Song)
            .OrderByDescending(us => us.AddedTime)
            .Select(us => us.Song)
            .Where(s => EF.Functions.ILike(s.Title, $"{titlePart}%"))
            .ToListAsync();
    }

    public async Task<UserAlbum> AddAlbumAsync(Guid userId, Guid albumId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var userAlbum = context.Set<UserAlbum>()
            .FirstOrDefault(a => a.UserId == userId && a.AlbumId == albumId);
        
        if (userAlbum is null)
            return null;

        userAlbum = new UserAlbum
        {
            UserId = userId,
            AlbumId = albumId,
            AddedTime = DateTime.UtcNow
        };
        
        await context.Set<UserAlbum>().AddAsync(userAlbum);
        
        return userAlbum;
    }

    public async Task<UserSong> AddSongAsync(Guid userId, Guid songId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var userSong = context.Set<UserSong>()
            .FirstOrDefault(s => s.UserId == userId && s.SongId == songId);
        
        if (userSong is null)
            return null;

        userSong = new UserSong
        {
            UserId = userId,
            SongId = songId,
            AddedTime = DateTime.UtcNow
        };
        
        await context.Set<UserSong>().AddAsync(userSong);
        
        return userSong;
    }
}