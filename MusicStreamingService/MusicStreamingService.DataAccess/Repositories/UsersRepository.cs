using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Context;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.DataAccess.Repositories.Interfaces;

namespace MusicStreamingService.DataAccess.Repositories;

public class UsersRepository : IUsersRepository
{
     private readonly MusicServiceDbContext _context;

    public UsersRepository(MusicServiceDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<IEnumerable<User>> FindAllAsync()
    {
        return await _context.Set<User>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> FindAllAsync(Expression<Func<User, bool>> predicate)
    {
        return await _context.Set<User>()
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<User?> FindByIdAsync(Guid id)
    {
        return await _context.Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public void Delete(User entity)
    {
        _context.Set<User>().Remove(entity);
    }

    public async Task<User?> SaveAsync(User entity)
    {
        var user = _context.Set<User>().FirstOrDefault(a => a.Id == entity.Id);
        
        if (user is not null) 
            return null;
        
        var result = await _context.Set<User>().AddAsync(entity);
        return result.Entity;
    }

    public User Update(User entity)
    {
        var result = _context.Set<User>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return result.Entity;
    }

    public async Task<IEnumerable<User>> FindByEmailAsync(string email)
    {
        return await _context.Set<User>()
            .AsNoTracking()
            .Where(a => a.Email == email)
            .ToListAsync();
    }

    public async Task<IEnumerable<Album>> FindAllAlbumsAsync(Guid userId)
    {
        return await _context.Set<UserAlbum>()
            .Where(ua => ua.UserId == userId)
            .Include(ua => ua.Album)
            .OrderByDescending(ua => ua.AddedTime)
            .Select(ua => ua.Album)
            .ToListAsync();
    }

    public async Task<IEnumerable<Album>> FindAllAlbumsByTitleAsync(Guid userId, string titlePart)
    {
        return await _context.Set<UserAlbum>()
            .Where(ua => ua.UserId == userId)
            .Include(ua => ua.Album)
            .OrderByDescending(ua => ua.AddedTime)
            .Select(ua => ua.Album)
            .Where(a => EF.Functions.ILike(a.Title, $"{titlePart}%"))
            .ToListAsync(); 
    }

    public async Task<IEnumerable<Song>> FindAllSongsAsync(Guid userId)
    {
        return await _context.Set<UserSong>()
            .Where(us => us.UserId == userId)
            .Include(us => us.Song)
            .OrderByDescending(us => us.AddedTime)
            .Select(us => us.Song)
            .ToListAsync();
    }

    public async Task<IEnumerable<Song>> FindAllSongsByTitleAsync(Guid userId, string titlePart)
    {
        return await _context.Set<UserSong>()
            .Where(us => us.UserId == userId)
            .Include(us => us.Song)
            .OrderByDescending(us => us.AddedTime)
            .Select(us => us.Song)
            .Where(s => EF.Functions.ILike(s.Title, $"{titlePart}%"))
            .ToListAsync();
    }

    public async Task<UserAlbum> AddAlbumAsync(Guid userId, Guid albumId)
    {
        var userAlbum = _context.Set<UserAlbum>()
            .FirstOrDefault(a => a.UserId == userId && a.AlbumId == albumId);
        
        if (userAlbum is null)
            return null;

        userAlbum = new UserAlbum
        {
            UserId = userId,
            AlbumId = albumId,
            AddedTime = DateTime.UtcNow
        };
        
        await _context.Set<UserAlbum>().AddAsync(userAlbum);
        
        return userAlbum;
    }

    public async Task<UserSong> AddSongAsync(Guid userId, Guid songId)
    {
        var userSong = _context.Set<UserSong>()
            .FirstOrDefault(s => s.UserId == userId && s.SongId == songId);
        
        if (userSong is null)
            return null;

        userSong = new UserSong
        {
            UserId = userId,
            SongId = songId,
            AddedTime = DateTime.UtcNow
        };
        
        await _context.Set<UserSong>().AddAsync(userSong);
        
        return userSong;
    }
}