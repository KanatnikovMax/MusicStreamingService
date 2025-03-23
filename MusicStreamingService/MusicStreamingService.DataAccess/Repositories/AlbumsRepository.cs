using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Context;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.DataAccess.Repositories.Interfaces;

namespace MusicStreamingService.DataAccess.Repositories;

public class AlbumsRepository : IAlbumsRepository
{
    private readonly IDbContextFactory<MusicServiceDbContext> _dbContextFactory;

    public AlbumsRepository(IDbContextFactory<MusicServiceDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<Album>> FindAllAsync()
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<Album>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Album>> FindAllAsync(Expression<Func<Album, bool>> predicate)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<Album>()
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<Album?> FindByIdAsync(Guid id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<Album>()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task DeleteAsync(Album entity)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        context.Set<Album>().Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<Guid?> SaveAsync(Album entity)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var album = context.Set<Album>().FirstOrDefault(a => a.Id == entity.Id);
        if (album is not null) 
            return null;
        
        var result = await context.Set<Album>().AddAsync(entity);
        await context.SaveChangesAsync();
        return result.Entity.Id;
    }

    public async Task<Guid?> UpdateAsync(Album entity)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var album = context.Set<Album>().FirstOrDefault(a => a.Id == entity.Id);
        if (album is null) 
            return null;
        
        var result = context.Set<Album>().Attach(entity);
        context.Entry(entity).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return result.Entity.Id;
    }

    public async Task<IEnumerable<Album>> FindByTitleAsync(string titlePart)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<Album>()
            .AsNoTracking()
            .Where(a => a.Title.Contains(titlePart))
            .ToListAsync();
    }

    public async Task<IEnumerable<Song>> FindAllSongsAsync(Guid albumId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var album = await context.Set<Album>()
            .AsNoTracking()
            .Include(a => a.Songs)
            .FirstOrDefaultAsync(a => a.Id == albumId);
        
        return album?.Songs
               ?? Enumerable.Empty<Song>();
    }
}