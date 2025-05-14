using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Postgres.Context;
using MusicStreamingService.DataAccess.Postgres.Entities;
using MusicStreamingService.DataAccess.Postgres.Repositories.Interfaces;

namespace MusicStreamingService.DataAccess.Postgres.Repositories;

public class SongsRepository : ISongsRepository
{
    private readonly MusicServiceDbContext _context;

    public SongsRepository(MusicServiceDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<CursorResponse<DateTime?, Song>> FindAllAsync(PaginationParams<DateTime?> request)
    {
        var songs = _context.Set<Song>()
            .Include(s => s.Artists)
            .AsNoTracking();
        
        if (request.Cursor is not null)
        {
            songs = songs.Where(s => s.CreatedAt >= request.Cursor);
        }

        var items = await songs.OrderBy(s => s.CreatedAt)
            .Take(request.PageSize + 1)
            .ToListAsync();
        
        var cursor = items.Count > request.PageSize ? items.LastOrDefault()?.CreatedAt : null;

        return new CursorResponse<DateTime?, Song>
        {
            Cursor = cursor,
            Items = items
        };
    }

    public async Task<IEnumerable<Song>> FindAllAsync(Expression<Func<Song, bool>> predicate)
    {
        return await _context.Set<Song>()
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<Song?> FindByIdAsync(Guid id)
    {
        return await _context.Set<Song>()
            .Include(s => s.Artists)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public void Delete(Song entity)
    {
        _context.Set<Song>().Remove(entity);
    }

    public async Task<Song?> SaveAsync(Song entity)
    {
        var song = _context.Set<Song>().FirstOrDefault(a => a.Id == entity.Id);
        
        if (song is not null) 
            return null;
        
        var result = await _context.Set<Song>().AddAsync(entity);
        return result.Entity;
    }

    public Song Update(Song entity)
    {
        var result = _context.Set<Song>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return result.Entity;
    }
    
    public async Task<Song?> FindByTitleAsync(string title)
    {
        return await _context.Set<Song>()
            .Include(s => s.Artists)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => EF.Functions.ILike(a.Title, title));
    }
    
    public async Task<CursorResponse<DateTime?, Song>> FindByTitlePartAsync(string titlePart, 
        PaginationParams<DateTime?> request)
    {
        var songs = _context.Set<Song>()
            .Include(s => s.Artists)
            .AsNoTracking()
            .Where(a => EF.Functions.ILike(a.Title, $"%{titlePart}%"));
        
        if (request.Cursor is not null)
        {
            songs = songs.Where(s => s.CreatedAt >= request.Cursor);
        }

        var items = await songs.OrderBy(s => s.CreatedAt)
            .Take(request.PageSize + 1)
            .ToListAsync();
        
        var cursor = items.Count > request.PageSize ? items.LastOrDefault()?.CreatedAt : null;
        
        return new CursorResponse<DateTime?, Song>
        {
            Cursor = cursor,
            Items = items
        };
    }
}