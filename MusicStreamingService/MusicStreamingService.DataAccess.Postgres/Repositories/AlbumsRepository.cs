using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Postgres.Context;
using MusicStreamingService.DataAccess.Postgres.Entities;
using MusicStreamingService.DataAccess.Postgres.Repositories.Interfaces;


namespace MusicStreamingService.DataAccess.Postgres.Repositories;

public class AlbumsRepository : IAlbumsRepository
{
    private readonly MusicServiceDbContext _context;

    public AlbumsRepository(MusicServiceDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<CursorResponse<DateTime?, Album>> FindAllAsync(PaginationParams<DateTime?> request)
    {
        var albums = _context.Set<Album>()
            .Include(a => a.Artists)
            .Include(a => a.Songs)!
            .ThenInclude(s => s.Artists)
            .AsNoTracking();
        
        if (request.Cursor is not null)
        {
            albums = albums.Where(s => s.ReleaseDate <= request.Cursor);
        }

        var items = await albums.OrderByDescending(s => s.ReleaseDate)
            .Take(request.PageSize + 1)
            .ToListAsync();
        
        var cursor = items.Count > request.PageSize ? items.LastOrDefault()?.ReleaseDate : null;
        
        return new CursorResponse<DateTime?, Album>
        {
            Cursor = cursor,
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
    
    public async Task<CursorResponse<DateTime?, Album>> FindByTitlePartAsync(string titlePart, 
        PaginationParams<DateTime?> request)
    {
        var albums = _context.Set<Album>()
            .Include(a => a.Artists)
            .Include(a => a.Songs)!
            .ThenInclude(s => s.Artists)
            .Where(a => EF.Functions.ILike(a.Title, $"%{titlePart}%"))
            .AsNoTracking();
        
        if (request.Cursor is not null)
        {
            albums = albums.Where(s => s.ReleaseDate <= request.Cursor);
        }

        var items = await albums.OrderByDescending(s => s.ReleaseDate)
            .Take(request.PageSize + 1)
            .ToListAsync();
        
        var cursor = items.Count > request.PageSize ? items.LastOrDefault()?.ReleaseDate : null;
        
        return new CursorResponse<DateTime?, Album>
        {
            Cursor = cursor,
            Items = items
        };
    }

    public async Task<CursorResponse<int?, Song>> FindAllSongsAsync(Guid albumId, PaginationParams<int?> request)
    {
        var album = await _context.Set<Album>()
            .Include(a => a.Songs)!
            .ThenInclude(s => s.Artists)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == albumId);

        if (album is null)
        {
            return new CursorResponse<int?, Song>
            {
                Cursor = null,
                Items = []
            };
        }

        var songs = album.Songs ?? [];
        
        if (request.Cursor is not null)
        {
            songs = songs.Where(s => s.TrackNumber >= request.Cursor).ToList();
        }

        var items = songs.OrderBy(s => s.TrackNumber)
            .Take(request.PageSize + 1)
            .ToList();
        
        var cursor = items.Count > request.PageSize ? items.LastOrDefault()?.TrackNumber : null;

        return new CursorResponse<int?,Song>
        {
            Cursor = cursor,
            Items = items
        };
    }
}