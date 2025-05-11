using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Postgres.Context;
using MusicStreamingService.DataAccess.Postgres.Entities;
using MusicStreamingService.DataAccess.Postgres.Repositories.Interfaces;

namespace MusicStreamingService.DataAccess.Postgres.Repositories;

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

    public async Task<CursorResponse<DateTime?, Album>> FindAllAlbumsAsync(Guid userId,
        PaginationParams<DateTime?> request)
    {
        var userAlbums = _context.Set<UserAlbum>()
            .Where(ua => ua.UserId == userId)
            .Include(ua => ua.Album)
            .ThenInclude(a => a.Artists)
            .AsNoTracking();

        if (!userAlbums.Any())
        {
            return new CursorResponse<DateTime?, Album>
            {
                Cursor = null,
                Items = []
            };
        }

        if (request.Cursor is not null)
        {
            userAlbums = userAlbums.Where(ua => ua.AddedTime <= request.Cursor);
        }

        var items = await userAlbums.OrderByDescending(ua => ua.AddedTime)
            .Take(request.PageSize + 1)
            .ToListAsync();
        
        var cursor = items.Count > request.PageSize ? items.LastOrDefault()?.AddedTime : null;
        
        return new CursorResponse<DateTime?, Album>
        {
            Cursor = cursor,
            Items = items.Take(request.PageSize)
                .Select(ua => ua.Album)
                .ToList(),
        };
    }

    public async Task<CursorResponse<DateTime?, Album>> FindAllAlbumsByTitleAsync(Guid userId, string titlePart,
        PaginationParams<DateTime?> request)
    {
        var userAlbums = _context.Set<UserAlbum>()
            .Where(ua => ua.UserId == userId)
            .Include(ua => ua.Album)
            .ThenInclude(a => a.Artists)
            .Where(ua => EF.Functions.ILike(ua.Album.Title, $"%{titlePart}%"))
            .AsNoTracking();

        if (!userAlbums.Any())
        {
            return new CursorResponse<DateTime?, Album>
            {
                Cursor = null,
                Items = []
            };
        }

        if (request.Cursor is not null)
        {
            userAlbums = userAlbums.Where(ua => ua.AddedTime <= request.Cursor);
        }

        var items = await userAlbums.OrderByDescending(ua => ua.AddedTime)
            .Take(request.PageSize + 1)
            .ToListAsync();
        
        var cursor = items.Count > request.PageSize ? items.LastOrDefault()?.AddedTime : null;
        
        return new CursorResponse<DateTime?, Album>
        {
            Cursor = cursor,
            Items = items.Take(request.PageSize)
                .Select(ua => ua.Album)
                .ToList(),
        };
    }

    public async Task<CursorResponse<DateTime?, Song>> FindAllSongsAsync(Guid userId, 
        PaginationParams<DateTime?> request)
    {
        var userSongs = _context.Set<UserSong>()
            .Where(ua => ua.UserId == userId)
            .Include(ua => ua.Song)
            .ThenInclude(s => s.Artists)
            .AsNoTracking();

        if (!userSongs.Any())
        {
            return new CursorResponse<DateTime?, Song>
            {
                Cursor = null,
                Items = []
            };
        }

        if (request.Cursor is not null)
        {
            userSongs = userSongs.Where(us => us.AddedTime <= request.Cursor);
        }

        var items = await userSongs.OrderByDescending(us => us.AddedTime)
            .Take(request.PageSize + 1)
            .ToListAsync();
        
        var cursor = items.Count > request.PageSize ? items.LastOrDefault()?.AddedTime : null;
        
        return new CursorResponse<DateTime?, Song>
        {
            Cursor = cursor,
            Items = items.Take(request.PageSize)
                .Select(us => us.Song)
                .ToList(),
        };
    }

    public async Task<CursorResponse<DateTime?, Song>> FindAllSongsByNameAsync(Guid userId, string namePart,
        PaginationParams<DateTime?> request)
    {
        var userSongs = _context.Set<UserSong>()
            .Where(ua => ua.UserId == userId)
            .Include(ua => ua.Song)
            .ThenInclude(s => s.Artists)
            .Where(us => EF.Functions.ILike(us.Song.Title, $"%{namePart}%") 
                         || us.Song.Artists.Any(a => EF.Functions.ILike(a.Name, $"%{namePart}%")))
            .AsNoTracking();

        if (!userSongs.Any())
        {
            return new CursorResponse<DateTime?, Song>
            {
                Cursor = null,
                Items = []
            };
        }

        if (request.Cursor is not null)
        {
            userSongs = userSongs.Where(us => us.AddedTime <= request.Cursor);
        }

        var items = await userSongs.OrderByDescending(us => us.AddedTime)
            .Take(request.PageSize + 1)
            .ToListAsync();
        
        var cursor = items.Count > request.PageSize ? items.LastOrDefault()?.AddedTime : null;
        
        return new CursorResponse<DateTime?, Song>
        {
            Cursor = cursor,
            Items = items.Take(request.PageSize)
                .Select(us => us.Song)
                .ToList(),
        };
    }

    public async Task<UserAlbum?> AddAlbumAsync(Guid userId, Guid albumId)
    {
        var userAlbum = _context.Set<UserAlbum>()
            .FirstOrDefault(a => a.UserId == userId && a.AlbumId == albumId);
        
        if (userAlbum is not null)
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
        
        if (userSong is not null)
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

    public async Task<UserAlbum?> FindAlbumByIdAsync(Guid userId, Guid albumId)
    {
        return await _context.Set<UserAlbum>()
            .FirstOrDefaultAsync(a => a.UserId == userId && a.AlbumId == albumId);
    }

    public async Task<UserSong?> FindSongByIdAsync(Guid userId, Guid songId)
    {
        return await _context.Set<UserSong>()
            .FirstOrDefaultAsync(a => a.UserId == userId && a.SongId == songId);
    }

    public void DeleteAlbum(UserAlbum album)
    {
        _context.Set<UserAlbum>().Remove(album);
    }

    public void DeleteSong(UserSong song)
    {
        _context.Set<UserSong>().Remove(song);
    }
}