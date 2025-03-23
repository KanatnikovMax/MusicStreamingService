using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Context;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.DataAccess.Repositories.Interfaces;

namespace MusicStreamingService.DataAccess.Repositories;

public class ArtistsRepository : IArtistsRepository
{
    private readonly IDbContextFactory<MusicServiceDbContext> _dbContextFactory;

    public ArtistsRepository(IDbContextFactory<MusicServiceDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<Artist>> FindAllAsync()
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<Artist>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Artist>> FindAllAsync(Expression<Func<Artist, bool>> predicate)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<Artist>()
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<Artist?> FindByIdAsync(Guid id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<Artist>()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Artist>> FindByNameAsync(string namePart)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<Artist>()
            .AsNoTracking()
            .Where(a => a.Name.Contains(namePart))
            .ToListAsync();
    }

    public async Task DeleteAsync(Artist entity)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        context.Set<Artist>().Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<Guid?> SaveAsync(Artist entity)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var artist = context.Set<Artist>().FirstOrDefault(a => a.Id == entity.Id);
        if (artist is not null) 
            return null;
        
        var result = await context.Set<Artist>().AddAsync(entity);
        await context.SaveChangesAsync();
        return result.Entity.Id;
    }

    public async Task<Guid?> UpdateAsync(Artist entity)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var artist = context.Set<Artist>().FirstOrDefault(a => a.Id == entity.Id);
        if (artist is null) 
            return null;
        
        var result = context.Set<Artist>().Attach(entity);
        context.Entry(entity).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return result.Entity.Id;
    }

    public async Task<IEnumerable<Album>> FindAllAlbumsAsync(Guid artistId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var artist = await context.Set<Artist>()
            .AsNoTracking()
            .Include(a => a.Albums)
            .FirstOrDefaultAsync(a => a.Id == artistId);
        
        if (artist is null)
            return Enumerable.Empty<Album>();
        
        return artist.Albums?.OrderByDescending(a => a.ReleaseDate)
            ?? Enumerable.Empty<Album>();
    }

    public async Task<IEnumerable<Song>> FindAllSongsAsync(Guid artistId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var artist = await context.Set<Artist>()
            .AsNoTracking()
            .Include(a => a.Songs)
            .FirstOrDefaultAsync(a => a.Id == artistId);
        
        return artist?.Songs
            ?? Enumerable.Empty<Song>();
    }

    public async Task<IEnumerable<Song>> FindAllSongsByTitleAsync(Guid artistId, string titlePart)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var artist = await context.Set<Artist>()
            .AsNoTracking()
            .Include(a => a.Songs)
            .FirstOrDefaultAsync(a => a.Id == artistId);
        
        if (artist is null)
            return Enumerable.Empty<Song>();
        
        return artist.Songs?.Where(s => s.Title.Contains(titlePart))
               ?? Enumerable.Empty<Song>();
    }
}