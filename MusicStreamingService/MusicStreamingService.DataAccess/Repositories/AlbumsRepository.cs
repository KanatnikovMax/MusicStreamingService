using System.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.BusinessLogic.Helpers;
using MusicStreamingService.DataAccess.Context;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.DataAccess.Repositories.Interfaces;
using Npgsql;
using NpgsqlTypes;

namespace MusicStreamingService.DataAccess.Repositories;

public class AlbumsRepository : IAlbumsRepository
{
    private readonly MusicServiceDbContext _context;

    public AlbumsRepository(MusicServiceDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<IEnumerable<Album>> FindAllAsync()
    {
        return await _context.Set<Album>()
            .Include(a => a.Artists)
            .Include(a => a.Songs)
            .AsNoTracking()
            .ToListAsync();
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
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task DeleteAsync(Album entity)
    {
        _context.Set<Album>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<Album?> SaveAsync(Album entity, IEnumerable<string> artistNames)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            if (entity.Id != Guid.Empty && await _context.Set<Album>()
                    .AnyAsync(a => a.Id == entity.Id))
            {
                await transaction.RollbackAsync();
                return null;
            }
            var album = await _context.Set<Album>()
                .Include(a => a.Artists)
                .FirstOrDefaultAsync(a => a.Title.ToLower() == entity.Title.ToLower());
            if (album is not null)
            {
                var artistIds = album.Artists
                    .Select(a => a.Id)
                    .OrderBy(id => id);

                var existingArtistIds = album.Artists
                    .Select(a => a.Id)
                    .OrderBy(id => id);
                
                var isDuplicate = artistIds
                    .OrderBy(id => id)
                    .SequenceEqual(existingArtistIds);
            
                if (isDuplicate)
                {
                    await transaction.RollbackAsync();
                    return null;
                }
            }
            
            var artists = await ProcessArtists(_context, artistNames);
        
            entity.Artists = artists;
            _context.Albums.Add(entity);
        
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        
            return entity;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static async Task<List<Artist>> ProcessArtists(DbContext context, IEnumerable<string> names)
    {
        var nameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in names)
        {
            var trimmedName = name.Trim();
            var normalized = trimmedName.ToLowerInvariant();
            nameMap.TryAdd(normalized, trimmedName);
        }
        var normalizedNames = nameMap.Keys.ToList();

        var existingArtists = await context.Set<Artist>()
            .Where(a => normalizedNames.Contains(a.Name.ToLower()))
            .ToListAsync();

        var existingNames = new HashSet<string>(
            existingArtists.Select(a => a.Name.ToLower()), 
            StringComparer.OrdinalIgnoreCase
        );

        var newArtists = normalizedNames
            .Where(n => !existingNames.Contains(n))
            .Select(n => new Artist { Name = nameMap[n] }) 
            .ToList();

        if (newArtists.Count > 0)
        {
            await context.Set<Artist>().AddRangeAsync(newArtists);
            await context.SaveChangesAsync();
        }

        return existingArtists.Concat(newArtists).ToList();
    }
    
    public async Task<Album> UpdateAsync(Album entity)
    {
        var result = _context.Set<Album>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
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
    
    public async Task<IEnumerable<Album>> FindByTitlePartAsync(string titlePart)
    {
        return await _context.Set<Album>()
            .Include(a => a.Artists)
            .Include(a => a.Songs)
            .AsNoTracking()
            .Where(a => EF.Functions.ILike(a.Title, $"%{titlePart}%"))
            .ToListAsync();
    }

    public async Task<IEnumerable<Song>> FindAllSongsAsync(Guid albumId)
    {
        var album = await _context.Set<Album>()
            .Include(a => a.Songs)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == albumId);

        if (album is null)
            return Enumerable.Empty<Song>();

        return album.Songs?.OrderBy(s => s.TrackNumber)
            ?? Enumerable.Empty<Song>();
    }
}