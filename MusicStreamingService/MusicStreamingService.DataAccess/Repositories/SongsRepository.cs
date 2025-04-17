using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Context;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.DataAccess.Repositories.Interfaces;

namespace MusicStreamingService.DataAccess.Repositories;

public class SongsRepository : ISongsRepository // TODO: сделать пагинацию
{
    private readonly MusicServiceDbContext _context;

    public SongsRepository(MusicServiceDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<IEnumerable<Song>> FindAllAsync()
    {
        return await _context.Set<Song>()
            .AsNoTracking()
            .ToListAsync();
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
            .AsNoTracking()
            .FirstOrDefaultAsync(a => EF.Functions.ILike(a.Title, title));
    }
    
    public async Task<IEnumerable<Song>> FindByTitlePartAsync(string titlePart)
    {
        return await _context.Set<Song>()
            .AsNoTracking()
            .Where(a => EF.Functions.ILike(a.Title, $"%{titlePart}%"))
            .ToListAsync();
    }
}