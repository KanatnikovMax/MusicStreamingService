using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Context;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.DataAccess.Repositories.Interfaces;

namespace MusicStreamingService.DataAccess.Repositories;

public class SongsRepository : ISongsRepository // TODO: сделать пагинацию
{
    private readonly IDbContextFactory<MusicServiceDbContext> _dbContextFactory;

    public SongsRepository(IDbContextFactory<MusicServiceDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<Song>> FindAllAsync()
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<Song>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Song>> FindAllAsync(Expression<Func<Song, bool>> predicate)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<Song>()
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<Song?> FindByIdAsync(Guid id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<Song>()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task DeleteAsync(Song entity)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        context.Set<Song>().Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<Song?> SaveAsync(Song entity)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var song = context.Set<Song>().FirstOrDefault(a => a.Id == entity.Id);
        
        if (song is not null) 
            return null;
        
        var result = await context.Set<Song>().AddAsync(entity);
        await context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<Song?> UpdateAsync(Song entity)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var result = context.Set<Song>().Attach(entity);
        context.Entry(entity).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return result.Entity;
    }
    
    public async Task<Song?> FindByTitleAsync(string title)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<Song>()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => EF.Functions.ILike(a.Title, title));
    }
    
    public async Task<IEnumerable<Song>> FindByTitlePartAsync(string titlePart)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<Song>()
            .AsNoTracking()
            .Where(a => EF.Functions.ILike(a.Title, $"%{titlePart}%"))
            .ToListAsync();
    }
}