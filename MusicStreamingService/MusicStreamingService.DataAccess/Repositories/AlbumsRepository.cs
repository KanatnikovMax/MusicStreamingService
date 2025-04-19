using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Context;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.DataAccess.Repositories.Interfaces;


namespace MusicStreamingService.DataAccess.Repositories;

public class AlbumsRepository : IAlbumsRepository
{
    private readonly MusicServiceDbContext _context;

    public AlbumsRepository(MusicServiceDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<PaginatedResponse<Album>> FindAllAsync(PaginationParams request)
    {
        var albums = _context.Set<Album>()
            .Include(a => a.Artists)
            .Include(a => a.Songs)!
            .ThenInclude(s => s.Artists)
            .AsNoTracking();
        
        if (request.Cursor is not null)
        {
            albums = albums.Where(s => s.CreatedAt >= request.Cursor);
        }

        var items = await albums.OrderBy(s => s.CreatedAt)
            .Take(request.PageSize + 1)
            .ToListAsync();

        return new PaginatedResponse<Album>
        {
            Cursor = items.LastOrDefault()?.CreatedAt,
            Items = items
        };
    }

    public async Task<IEnumerable<Album>> FindAllAsync(Expression<Func<Album, bool>> predicate)
    {
        return await _context.Set<Album>()
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<Album?> FindByIdAsync(Guid id)
    {
        return await _context.Set<Album>()
            .Include(a => a.Artists)
            .Include(a => a.Songs)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public void Delete(Album entity)
    {
        _context.Set<Album>().Remove(entity);
    }

    public async Task<Album?> SaveAsync(Album entity)
    {
        if (entity.Id != Guid.Empty && await _context.Set<Album>()
                .AnyAsync(a => a.Id == entity.Id))
        {
            return null;
        }

        var result = await _context.Albums.AddAsync(entity);
        return result.Entity;
    }
        
    public Album Update(Album entity)
    {
        var result = _context.Set<Album>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return result.Entity;
    }

    public async Task<Album?> FindByTitleAsync(string title)
    {
        return await _context.Set<Album>()
            .Include(a => a.Artists)
            .Include(a => a.Songs)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => EF.Functions.ILike(a.Title, title));
    }
    
    public async Task<PaginatedResponse<Album>> FindByTitlePartAsync(string titlePart, PaginationParams request)
    {
        var albums = _context.Set<Album>()
            .Include(a => a.Artists)
            .Include(a => a.Songs)!
            .ThenInclude(s => s.Artists)
            .Where(a => EF.Functions.ILike(a.Title, $"%{titlePart}%"))
            .AsNoTracking();
        
        if (request.Cursor is not null)
        {
            albums = albums.Where(s => s.CreatedAt >= request.Cursor);
        }

        var items = await albums.OrderBy(s => s.CreatedAt)
            .Take(request.PageSize + 1)
            .ToListAsync();

        return new PaginatedResponse<Album>
        {
            Cursor = items.LastOrDefault()?.CreatedAt,
            Items = items
        };
    }

    public async Task<PaginatedResponse<Song>> FindAllSongsAsync(Guid albumId, PaginationParams request)
    {
        var album = await _context.Set<Album>()
            .Include(a => a.Songs)!
            .ThenInclude(s => s.Artists)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == albumId);

        if (album is null)
        {
            return new PaginatedResponse<Song>
            {
                Cursor = null,
                Items = []
            };
        }

        var songs = album.Songs ?? [];
        
        if (request.Cursor is not null)
        {
            songs = songs.Where(s => s.CreatedAt >= request.Cursor).ToList();
        }

        var items = songs.OrderBy(s => s.CreatedAt)
            .Take(request.PageSize + 1)
            .ToList();

        return new PaginatedResponse<Song>
        {
            Cursor = items.LastOrDefault()?.CreatedAt,
            Items = items
        };
    }
}