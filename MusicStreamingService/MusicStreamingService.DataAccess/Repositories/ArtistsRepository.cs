using System.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Context;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.DataAccess.Repositories.Interfaces;

namespace MusicStreamingService.DataAccess.Repositories;

public class ArtistsRepository : IArtistsRepository 
{
    private readonly MusicServiceDbContext _context;

    public ArtistsRepository(MusicServiceDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<IEnumerable<Artist>> FindAllAsync()
    {
        return await _context.Set<Artist>()
            .Include(a => a.Albums)
            .Include(a => a.Songs)
            .ToListAsync();
    }

    public async Task<IEnumerable<Artist>> FindAllAsync(Expression<Func<Artist, bool>> predicate)
    {
        return await _context.Set<Artist>()
            .Include(a => a.Albums)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<Artist?> FindByIdAsync(Guid id)
    {
        return await _context.Set<Artist>()
            .Include(a => a.Albums)!
            .ThenInclude(a => a.Artists)
            .Include(a => a.Songs)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
    
    public async Task<Artist?> FindByNameAsync(string name)
    {
        return await _context.Set<Artist>()
            .Include(a => a.Albums)!
            .ThenInclude(a => a.Artists)
            .Include(a => a.Songs)
            .FirstOrDefaultAsync(a => EF.Functions.ILike(a.Name, name));
   }
    
    public async Task<IEnumerable<Artist>> FindByNamePartAsync(string namePart)
    {
        return await _context.Set<Artist>()
            .Include(a => a.Albums)! 
            .ThenInclude(album => album.Artists) 
            .Include(a => a.Songs)
            .Where(a => EF.Functions.ILike(a.Name, $"%{namePart}%")) 
            .ToListAsync();
    }

    public async Task DeleteAsync(Artist entity)
    {
        _context.Set<Artist>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<Artist?> SaveAsync(Artist entity)
    {
        var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            var artist = _context.Set<Artist>().FirstOrDefault(a => a.Id == entity.Id 
                                                                   || a.Name.ToLower() == entity.Name.ToLower());
            if (artist is not null)
            {
                await transaction.RollbackAsync();
                return null;
            }
            
            var result = await _context.Set<Artist>().AddAsync(entity);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return result.Entity;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Artist> UpdateAsync(Artist entity)
    {
        var result = _context.Set<Artist>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<IEnumerable<Album>> FindAllAlbumsAsync(Guid artistId)
    {
        var artist = await _context.Set<Artist>()
            .Include(a => a.Albums)!
            .ThenInclude(a => a.Artists)
            .FirstOrDefaultAsync(a => a.Id == artistId);
        
        if (artist is null)
            return Enumerable.Empty<Album>();
        
        return artist.Albums?.OrderByDescending(a => a.ReleaseDate)
            ?? Enumerable.Empty<Album>();
    }

    public async Task<IEnumerable<Song>> FindAllSongsAsync(Guid artistId)
    {
        var artist = await _context.Set<Artist>()
            .AsNoTracking()
            .Include(a => a.Songs)
            .FirstOrDefaultAsync(a => a.Id == artistId);
        
        return artist?.Songs
            ?? Enumerable.Empty<Song>();
    }

    public async Task<IEnumerable<Song>> FindAllSongsByTitleAsync(Guid artistId, string titlePart)
    {
        var artist = await _context.Set<Artist>()
            .AsNoTracking()
            .Include(a => a.Songs)
            .FirstOrDefaultAsync(a => a.Id == artistId);
        
        if (artist is null)
            return Enumerable.Empty<Song>();
        
        return artist.Songs?.Where(s => s.Title.IndexOf(titlePart, StringComparison.OrdinalIgnoreCase) >= 0)
               ?? Enumerable.Empty<Song>();
    }
}